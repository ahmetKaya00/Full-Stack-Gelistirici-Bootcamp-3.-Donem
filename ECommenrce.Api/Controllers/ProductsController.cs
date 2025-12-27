using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using Ecommenrce.Api.Data;
using Ecommenrce.Api.Dtos.Products;
using Ecommenrce.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommenrce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IWebHostEnvironment _env;

        public ProductsController(ApplicationDbContext db,UserManager<ApplicationUser> userManager,IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
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
                jwt.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ??
                jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;

            return string.IsNullOrWhiteSpace(email) ? null : email;
        }

        private async Task<(ApplicationUser? user, SellerProfile? sellerProfile)> GetCurrentSellerAsync()
        {
            var email = GetEmailFromAuthorizationHeader();
            if(email == null)
                return(null,null);
            
            var user = await _userManager.Users.Include(u => u.SellerProfile).FirstOrDefaultAsync(u=>u.Email == email);

            if(user == null)
                return(null,null);
            
            var roles = await _userManager.GetRolesAsync(user);
            if(!roles.Contains("Seller"))
                return(null,null);
            
            if(user.SellerProfile == null || user.SellerProfile.Status != SellerStatus.Approved)
                return(user, null);
            
            return(user,user.SellerProfile);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _db.Products.Where(p=>p.IsPublished).Include(p=>p.Category).Include(p=>p.SellerProfile).Select(p=>new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                CategoryName = p.Category.Name,
                SellerShopName = p.SellerProfile.ShopName,
            }).ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>>GetById(int id)
        {
            var p = await _db.Products.Include(x=>x.Category).Include(x=>x.SellerProfile).FirstOrDefaultAsync(x=>x.Id == id && x.IsPublished);

            if(p==null)
                return NotFound("Ürün Bulunamadı");
            
            var dto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                CategoryName = p.Category.Name,
                SellerShopName = p.SellerProfile.ShopName,
            };
            return Ok(dto);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> MyProducts()
        {
            var (user, sellerProfile) = await GetCurrentSellerAsync();
            if(user == null)
                return Unauthorized("Bu endpoint sadece Seller içindir veya token geçersiz.");
            
            if(sellerProfile == null)
                return BadRequest("Onaylı bir satıcı profiliniz bulunamadı!");
            
            var products = await _db.Products.Where(p=>p.SellerProfileId == sellerProfile.Id).Include(p=>p.Category).Select(p=>new ProductDto{
                Id = p.Id,
                Name = p.Name,
                Stock = p.Stock,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                CategoryName = p.Category.Name,
                SellerShopName = p.SellerProfile.ShopName,
        }).ToListAsync();
        return Ok(products);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult>Create([FromForm] CreateProductDto dto)
        {
            var(user,sellerProfile) = await GetCurrentSellerAsync();

            if(user == null)
                return Unauthorized("Bu endpoint sadece Seller içindir veya token geçersiz.");

            if(sellerProfile == null)
                return BadRequest("Onaylı bir satıcı profiliniz bulunmamaktadır.");
            
            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if(category == null)
                return BadRequest("Kategori bulunamadı.");
            
            string? finalImageUrl = dto.ImageUrl;

            if(dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var webRoot = _env.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot");
                }
                var uploadRoot = Path.Combine(webRoot, "uploads","products");
                Directory.CreateDirectory(uploadRoot);

                var ext =  Path.GetExtension(dto.ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadRoot,fileName);

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                finalImageUrl = $"/uploads/products/{fileName}";
            }
            var products = new Product
            {
                Name = dto.Name,
                Stock = dto.Stock,
                Price = dto.Price,
                ImageUrl = finalImageUrl,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                SellerProfileId = sellerProfile.Id,
                IsPublished = true
            };

            _db.Products.Add(products);
            await _db.SaveChangesAsync();
            return Ok("Ürün Başarıyla Eklendi!");
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult>Update(int id,[FromForm] CreateProductDto dto)
        {
            var(user,sellerProfile) = await GetCurrentSellerAsync();

            if(user == null)
                return Unauthorized("Bu endpoint sadece Seller içindir veya token geçersiz.");

            if(sellerProfile == null)
                return BadRequest("Onaylı bir satıcı profiliniz bulunmamaktadır.");
            
            var products = await _db.Products.FirstOrDefaultAsync(p=>p.Id == id);
            if(products == null)
                return NotFound("Ürün bulunamadı");
            
            if(products.SellerProfileId != sellerProfile.Id)
                return Forbid("Bu ürünü güncellemeye yetkin yok");
            
            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if(category == null)
                return BadRequest("Kategori Bulunamadı");
            
            products.Name = dto.Name;
            products.Stock = dto.Stock;
            products.Price = dto.Price;
            products.Description = dto.Description;
            products.CategoryId = dto.CategoryId;

            string? finalImageUrl = dto.ImageUrl;

            if(dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var webRoot = _env.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot");
                }
                var uploadRoot = Path.Combine(webRoot, "uploads","products");
                Directory.CreateDirectory(uploadRoot);

                var ext =  Path.GetExtension(dto.ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadRoot,fileName);

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                finalImageUrl = $"/uploads/products/{fileName}";
            }

            products.ImageUrl = finalImageUrl;

            await _db.SaveChangesAsync();
            return Ok("Ürün Güncellendi");
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>Delete(int id)
        {
            var email = GetEmailFromAuthorizationHeader();
            if(email == null)
                return Unauthorized("Authorization header yok veya yoken geçersiz!");

            var user = await _userManager.Users.Include(u=>u.SellerProfile).FirstOrDefaultAsync(u=>u.Email == email);
            if(user == null)
                return Unauthorized("Token içindeki kullanıcı sistemde bulunamadı");
            
            var roles = await _userManager.GetRolesAsync(user);
            var products = await _db.Products.FirstOrDefaultAsync(p=>p.Id == id);
            if(products == null)
                return NotFound("Ürün bulunamadı");

            var isAdmin = roles.Contains("Admin");
            var isOwnerSeller = user.SellerProfile != null && user.SellerProfile.Id == products.SellerProfileId;

            if(!isAdmin && !isOwnerSeller)
                return Forbid("Bu ürünü silmeye yetkin yok");
            
            _db.Products.Remove(products);
            await _db.SaveChangesAsync();

            return Ok("Ürün silindi");

        }
    }
}