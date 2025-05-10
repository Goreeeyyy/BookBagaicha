namespace BookBagaicha.Models.Dto
{
    public class PlaceOrderRequest
    {
        public Guid CartId { get; set; }
        public decimal? AppliedDiscount { get; set; }
    }
}
