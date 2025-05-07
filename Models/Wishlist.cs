using System.ComponentModel.DataAnnotations;
namespace BookBagaicha.Models
{
    public class Wishlist
    {
        [Key]
        public Guid WishlistId { get; set; }
        public required long UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}