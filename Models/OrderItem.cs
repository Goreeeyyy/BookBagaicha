using System;

namespace BookBagaicha.Models
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }

        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}