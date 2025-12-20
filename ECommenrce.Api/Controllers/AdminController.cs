using System.IdentityModel.Tokens.Jwt;                     // ✔ JWT token çözümlemek için
using System.Security.Claims;                             // ✔ Claim tiplerine erişim için
using Ecommenrce.Api.Models;
using Ecommenrce.Api.Data;
                             // ✔ ApplicationUser ve SellerProfile erişimi
using Microsoft.AspNetCore.Identity;                      // ✔ Kullanıcı & rol yönetimi
using Microsoft.AspNetCore.Mvc;                           // ✔ Controller için gerekli attribute'lar
using Microsoft.EntityFrameworkCore;                      // ✔ Asenkron sorgular için

namespace ECommerce.Api.Controllers
{
    [ApiController]                                       // ✔ Controller davranışlarını otomatik hale getirir
    [Route("api/[controller]")]                           // ✔ Endpoint route: /api/Admin
    // DİKKAT: [Authorize] yok. Token validation manuel yapılıyor.
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;         // ✔ DB erişim nesnesi
        private readonly UserManager<ApplicationUser> _userManager; // ✔ Kullanıcı ve rol işlemleri

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ---------------------------------------------------------
        // 🔹 Authorization header'dan JWT'yi alıp email claim'ini çöz
        // ---------------------------------------------------------
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

        // ---------------------------------------------------------
        // 🔹 Token + Role kontrolü → Admin mi?
        // ---------------------------------------------------------
        private async Task<ApplicationUser?> GetCurrentAdminAsync()
        {
            var email = GetEmailFromAuthorizationHeader();
            if (email == null)
                return null;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))                    // Rol kontrolü
                return null;

            return user;                                     // Admin kullanıcı döner
        }

        // ---------------------------------------------------------
        // 📌 Bekleyen satıcıları listele
        // GET: /api/Admin/pending-sellers
        // ---------------------------------------------------------
        [HttpGet("pending-sellers")]
        public async Task<IActionResult> GetPendingSellers()
        {
            var admin = await GetCurrentAdminAsync();
            if (admin == null)
                return Unauthorized("Bu endpoint sadece Admin içindir veya token geçersiz.");

            // Status = Pending olan satıcıları getir
            var sellers = await _db.SellerProfiles
                .Include(s => s.User)                        // User bilgisi de lazım
                .Where(s => s.Status == SellerStatus.Pending)
                .Select(s => new
                {
                    s.Id,
                    s.ShopName,
                    s.Description,
                    UserEmail = s.User.Email                 // Admin panelde göstermek için
                })
                .ToListAsync();

            return Ok(sellers);
        }

        // ---------------------------------------------------------
        // 📌 Satıcı onayla → Rol ata
        // POST: /api/Admin/approve-seller/{id}
        // ---------------------------------------------------------
        [HttpPost("approve-seller/{id:int}")]
        public async Task<IActionResult> ApproveSeller(int id)
        {
            var admin = await GetCurrentAdminAsync();
            if (admin == null)
                return Unauthorized("Bu endpoint sadece Admin içindir veya token geçersiz.");

            // Satıcıyı getir
            var profile = await _db.SellerProfiles
                .Include(s => s.User)                        // Role eklemek için kullanıcıya ihtiyaç var
                .FirstOrDefaultAsync(s => s.Id == id);

            if (profile == null)
                return NotFound("Satıcı profili bulunamadı.");

            // Durum güncelle
            profile.Status = SellerStatus.Approved;
            await _db.SaveChangesAsync();

            // Kullanıcıya SELLER rolü ver
            await _userManager.AddToRoleAsync(profile.User, "Seller");

            return Ok("Satıcı profili onaylandı ve rol atandı.");
        }
    }
}