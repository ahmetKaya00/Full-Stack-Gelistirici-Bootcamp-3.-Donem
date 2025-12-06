namespace Ecommenrce.Api.Dtos.Orders
{
    public class OrderItemDto
    {
        public int Id{get;set;}
        public string ProductName {get;set;} = default!;
        public int Quantity {get;set;}
        public decimal UnitPrice{get;set;}
        public decimal LineTotal => UnitPrice * Quantity;
    }
    public class OrderDto
    {
        public int Id {get;set;}
        public DateTime CreatedAt{get;set;}
        public decimal TotalPrice{get;set;}
        public string Status{get;set;} = default!;
        public List<OrderItemDto> Items {get;set;} = default!;
    }
}