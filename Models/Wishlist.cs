using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBagaicha.Models
{
    public class Wishlist
    {
        [Key]
        public Guid WishlistId { get; set; }

        [Required]
        public long UserId { get; set; }


        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }

}
