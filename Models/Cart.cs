using System;
using System.Collections.Generic;

namespace BookBagaicha.Models
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public long UserId { get; set; }
        public decimal CartTotal { get; set; }

        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}