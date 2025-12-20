using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommenrce.Api.Data;
using Ecommenrce.Api.Dtos;
using Ecommenrce.Api.Dtos.Profile;
using Ecommenrce.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommenrce.Api.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser>_userManager;

        public ProfileController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db; _userManager = userManager;
        } 

        private string? GetEmailFromAuthorizationHeader()
        {
            // Authorization header var mı?
            if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
                return null;

            var authHeader = authHeaderValues.ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
                return null;

            const string bearerPrefix = "Bearer ";
            // Header "Bearer <token>" formatında olmalı
            if (!authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                return null;

            // Sadece token kısmını al
            var token = authHeader[bearerPrefix.Length..].Trim();
            if (string.IsNullOrWhiteSpace(token))
                return null;

            JwtSecurityToken jwt;
            try
            {
                // Token'ı parse et
                var handler = new JwtSecurityTokenHandler();
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                return null; // Token geçersizse email yok döner
            }

            // Email claim'ini farklı claim adlarına göre arar (güvenli yöntem)
            var email =
                jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ??
                jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            return string.IsNullOrWhiteSpace(email) ? null : email;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var email = GetEmailFromAuthorizationHeader();
            if(email == null)
            {
                return Unauthorized("Authorization header yok, Bearer token yok veya token parse edilemedi.");
            }

            var user = await _userManager.Users.Include(u=>u.SellerProfile).FirstOrDefaultAsync(u=>u.Email == email);

            if(user == null)
            {
                return Unauthorized("Token içindeki kullanıcı sistemde bulunamadı!");
            }
            SellerProfileDto? sellerDto = null;
            if(user.SellerProfile != null)
            {
                
            sellerDto = new SellerProfileDto
            {
                Id = user.SellerProfile.Id,
                ShopName = user.SellerProfile.ShopName,
                Description = user.SellerProfile.Description,
                Status = user.SellerProfile.Status.ToString()
            };
            }
            return Ok(new
            {
               user.FullName,
               user.Email,
               SellerProfile = sellerDto 
            });
        }
        [HttpPost("become-seller")]
        public async Task<IActionResult>BecomeSeller([FromBody] CreateSellerProfileDto dto)
        {
            var email = GetEmailFromAuthorizationHeader();
            if(email == null)
            {
                return Unauthorized("Authorization header yok, Bearer token yok veya token parse edilemedi.");
            }

            var user = await _userManager.Users.Include(u=>u.SellerProfile).FirstOrDefaultAsync(u=>u.Email == email);

            if(user == null)
            {
                return Unauthorized("Token içindeki kullanıcı sistemde bulunamadı!");
            }

            if(user.SellerProfile != null)
            {
                return BadRequest("Zaten satıcı profiliniz veya başvurunuz var.");
            }
            var profile = new SellerProfile
            {
                UserId = user.Id,
                ShopName = dto.ShopName,
                Description = dto.Description,
                Status = SellerStatus.Pending
            };
            _db.SellerProfiles.Add(profile);
            await _db.SaveChangesAsync();
            return Ok("Satıcı Başvurunuz alındı. Admin onayını bekliyor.");
        }
    }
}