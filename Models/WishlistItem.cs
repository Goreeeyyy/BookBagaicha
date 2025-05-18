using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBagaicha.Models
{

    public class WishlistItem
    {
        [Key]
        public Guid WishlistItemId { get; set; }

        [Required]
        public Guid WishlistId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        [ForeignKey("WishlistId")]
        public Wishlist Wishlist { get; set; } = null!;

        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;
    }
}