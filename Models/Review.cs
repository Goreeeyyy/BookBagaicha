using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookBagaicha.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; } // Assuming your User ID is of type long
        public User User { get; set; }

        [Range(1, 5)]
        public int Ratings { get; set; }

        public string? Comments { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}

