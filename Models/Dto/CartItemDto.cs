namespace BookBagaicha.Models.Dto
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public bool OnSale { get; set; }
        public decimal SalePercentage { get; set; }
        public string? Category { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
    }
}