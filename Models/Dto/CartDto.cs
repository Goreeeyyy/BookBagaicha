 namespace BookBagaicha.Models.Dto
    {
        public class CartDto
        {
            public Guid CartId { get; set; }
            public long UserId { get; set; }
            public decimal CartTotal { get; set; }
            public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        }
    }
