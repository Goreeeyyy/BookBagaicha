using System.ComponentModel.DataAnnotations;
namespace BookBagaicha.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }

        // Foreign key to User
        public long UserId { get; set; }

        // Cart total
        public decimal CartTotal { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}