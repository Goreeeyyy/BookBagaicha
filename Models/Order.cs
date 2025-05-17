using System;
using System.Collections.Generic;

namespace BookBagaicha.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public long UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } 
        public decimal TotalPrice { get; set; }
        public string ClaimCode { get; set; } // Unique code for order identification
        public decimal AppliedDiscount { get; set; }
        public bool AppliedDiscountIsLoyalty { get; set; }
        public bool ConfirmationEmailSent { get; set; }

        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}