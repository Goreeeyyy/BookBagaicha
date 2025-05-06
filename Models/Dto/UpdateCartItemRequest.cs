namespace BookBagaicha.Models.Dto
{
    public class UpdateCartItemRequest
    {
        public required Guid CartItemId { get; set; }
        public required int Quantity { get; set; }
    }
}