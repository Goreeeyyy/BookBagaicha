using System;

namespace BookBagaicha.Models
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public Guid CartId { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }

        // Navigation properties
        public Cart Cart { get; set; }
        public Book Book { get; set; }
    }
}