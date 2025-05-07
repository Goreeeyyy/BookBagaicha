using System.ComponentModel.DataAnnotations;
namespace BookBagaicha.Models
{
    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }

        // Foreign keys
        public Guid CartId { get; set; }
        public Guid BookId { get; set; }

        // Item details
        public int Quantity { get; set; }

        // Prices at time of adding to cart
        public decimal Price { get; set; }

        // Navigation properties
        public Cart Cart { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}