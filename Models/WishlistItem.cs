using System.ComponentModel.DataAnnotations;
namespace BookBagaicha.Models
{
    public class WishlistItem
    {
        [Key]
        public Guid WishlistItemId { get; set; }
        public Guid WishlistId { get; set; }
        public Guid BookId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Wishlist Wishlist { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}