namespace Ecommenrce.Api.Dtos
{
    public class SellerProfileDto
    {
        public int Id {get;set;}
        public string ShopName {get;set;} = default!;
        public string? Description {get;set;}
        public string Status {get;set;} = default!;
    }
}