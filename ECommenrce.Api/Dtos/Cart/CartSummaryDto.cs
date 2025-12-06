namespace Ecommenrce.Api.Dtos.Cart
{
    
    public class CartSummaryDto
    {
        public List<CartItemDto> Items {get;set;} = new();
        public decimal TotalPrice {get;set;}
    }
}