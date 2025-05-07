namespace BookBagaicha.Models.Dto
{
    public class ReviewDTO
    {
        
            public int ReviewId { get; set; }
            public Guid BookId { get; set; }
            public long UserId { get; set; }
            public int Ratings { get; set; }
            public string Comments { get; set; }
            public DateTime ReviewDate { get; set; }
            public string UserDisplayName { get; set; } // To display the user's name/username
        
    }
}
