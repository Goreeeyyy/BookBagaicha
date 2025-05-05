using System.ComponentModel.DataAnnotations;

namespace BookBagaicha.Models
{
    public class Book
    {
        [Key]
        public Guid BookId { get; set; }
        public required string Title { get; set; }
        public required string ISBN { get; set; }
        public required decimal Price { get; set; }
        public required string Summary { get; set; }
        public required string Language { get; set; }
        public  required string Format { get; set; }
        public  DateTime PublicationDate { get; set; }
        public decimal Quantity { get; set; }
        public Boolean OnSale { get; set; }
        public decimal SalePercntage { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
         
        public  string? Category {  get; set; }
        public string? Image {  get; set; }

        public int PublisherId { get; set; }

        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>(); // Added Genres navigation property


    }
}
