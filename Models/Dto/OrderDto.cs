namespace BookBagaicha.Models.Dto
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public long UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string ClaimCode { get; set; }
        public decimal AppliedDiscount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

}