namespace BookBagaicha.Models.Dto
{
    public class WishlistDto
    {
        public Guid WishlistId { get; set; }
        public long UserId { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new List<WishlistItemDto>();
    }

}