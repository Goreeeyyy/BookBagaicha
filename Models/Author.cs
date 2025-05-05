using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookBagaicha.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}