namespace NewEra_Cash___Carry.Core.DTOs.Payment
{
    public class RefundDto
    {
        public string RefundId { get; set; }        // Stripe Refund ID
        public string PaymentIntentId { get; set; } // Associated Payment Intent ID
        public decimal Amount { get; set; }         // Refund Amount
        public string Status { get; set; }          // Refund Status (e.g., Refunded, Failed)
        public DateTime RefundDate { get; set; }    // Date of Refund
    }
}
