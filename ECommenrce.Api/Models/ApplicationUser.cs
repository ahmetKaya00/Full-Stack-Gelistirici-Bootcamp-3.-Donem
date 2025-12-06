using Microsoft.AspNetCore.Identity;

namespace Ecommenrce.Api.Models
{
    public class ApplicationUser : IdentityUser {
        public string? FullName{get;set;}
        public SellerProfile? SellerProfile{get;set;}
    }
    
}