namespace BookBagaicha.Models.Dto
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; } = null!;
        public decimal CartTotal { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}