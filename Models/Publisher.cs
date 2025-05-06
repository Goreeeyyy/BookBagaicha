using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookBagaicha.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }

        [Required]
        public required string PublisherName { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}