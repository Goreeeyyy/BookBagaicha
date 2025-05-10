using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookBagaicha.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
