namespace Ecommenrce.Api.Dtos.Products
{
    
    public class CreateProductDto
    {
        public string Name {get;set;} = default!;
        public int Stock {get;set;}
        public decimal Price {get;set;}
        public string? Description {get;set;}
        public int CategoryId {get;set;}
        public string? ImageUrl {get;set;}
        public IFormFile? ImageFile {get;set;}
    }
}