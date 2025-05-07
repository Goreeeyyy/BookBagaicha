namespace BookBagaicha.Models.Dto
{
    public class AddToCartRequest
    {
        public required Guid BookId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
