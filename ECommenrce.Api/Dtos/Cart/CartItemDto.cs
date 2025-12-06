namespace Ecommenrce.Api.Dtos.Cart
{
    public class CartItemDto
    {
        public int ProductId {get;set;}
        public string ProductName {get;set;} = default!;
        public decimal UnitPrice {get;set;}
        public int Quantity {get;set;}
        public decimal LineTotal => UnitPrice * Quantity;
        public string? ImageUrl {get;set;}
    }
}