namespace Ecommenrce.Api.Models
{
    public class Order
    {
        public int Id {get;set;}
        public string UserId{get;set;}=default!;
        public ApplicationUser User{get;set;} = default!;
        public decimal TotalPrice {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
        public OrderStatus Status {get;set;} = OrderStatus.Pending;
        public ICollection<OrderItem>Items {get;set;} = new List<OrderItem>();

    }
}