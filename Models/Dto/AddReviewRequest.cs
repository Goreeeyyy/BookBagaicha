using System.ComponentModel.DataAnnotations;

namespace BookBagaicha.Models.Dto
{
    public class AddReviewRequest
    {
        public Guid BookId { get; set; }
        [Range(1, 5)]
        public int Ratings { get; set; }
        public string? Comments { get; set; }
    }
}
