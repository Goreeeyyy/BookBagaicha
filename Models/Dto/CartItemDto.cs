namespace BookBagaicha.Models.Dto
{
    public class CartItemDto
    {
        public Guid CartItemId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public bool OnSale { get; set; }
        public decimal SalePercentage { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
    }
}