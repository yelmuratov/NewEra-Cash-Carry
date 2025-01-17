namespace NewEra_Cash___Carry.Core.DTOs.Payment
{
    public class PaymentDto
    {
        public string PaymentIntentId { get; set; } // Stripe Payment Intent ID
        public decimal TotalAmount { get; set; }    // Total Amount Paid
        public string Currency { get; set; }        // Currency Code (e.g., USD)
        public string Status { get; set; }          // Payment Status (e.g., Paid, Failed)
        public DateTime PaymentDate { get; set; }   // Date of Payment
    }
}
