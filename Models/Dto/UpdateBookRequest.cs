using System.ComponentModel.DataAnnotations;

namespace BookBagaicha.Models.Dto
{
    public class UpdateBookRequest
    {
        [Required] public Guid BookId { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string ISBN { get; set; }
        public required decimal Price { get; set; }
        [Required] public string Summary { get; set; }
        [Required] public string Language { get; set; }
        [Required] public string Format { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Quantity { get; set; }
        public bool OnSale { get; set; }
        public decimal SalePercntage { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public string? Category { get; set; }
        public IFormFile? ImageFile { get; set; }
        [Required] public string PublisherName { get; set; } // Required for updating publisher
        public List<string> Authors { get; set; } = new List<string>();
        public List<string> Genres { get; set; } = new List<string>();
    }
}

