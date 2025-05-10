namespace BookBagaicha.Models.Dto
{
    public class OrderItemDto
    {
        public Guid OrderItemId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public string ISBN { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
    }

}
