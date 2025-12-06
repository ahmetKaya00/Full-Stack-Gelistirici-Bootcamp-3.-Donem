namespace Ecommenrce.Api.Models
{
    public class Product
    {
        public int Id {get;set;}
        public string Name {get;set;} = default!;
        public int Stock {get;set;}
        public decimal Price {get;set;}
        public string? ImageUrl {get;set;}
        public string? Description {get;set;}
        public int CategoryId {get;set;}
        public Category Category {get;set;} = default!;
        public int SellerProfileId {get;set;}
        public SellerProfile SellerProfile {get;set;} =default!;
        public bool IsPublished {get;set;} = true;
    }
}