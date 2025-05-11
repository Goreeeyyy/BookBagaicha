namespace BookBagaicha.Models.Dto
{
    public class OrderSummaryDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string ClaimCode { get; set; }
        public int ItemCount { get; set; }
    }
}
