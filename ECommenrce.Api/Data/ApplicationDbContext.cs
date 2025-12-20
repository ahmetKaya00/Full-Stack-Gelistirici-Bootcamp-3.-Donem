using Ecommenrce.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommenrce.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options){}

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                    .HasOne(u=>u.SellerProfile)
                    .WithOne(s=>s.User)
                    .HasForeignKey<SellerProfile>(s=>s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Category>()
                    .HasMany(c=>c.Products)
                    .WithOne(p=>p.Category)
                    .HasForeignKey(p=>p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<SellerProfile>()
                    .HasMany(s=>s.Products)
                    .WithOne(p=>p.SellerProfile)
                    .HasForeignKey(p=>p.SellerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}