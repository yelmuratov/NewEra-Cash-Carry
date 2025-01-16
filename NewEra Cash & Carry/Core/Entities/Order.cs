using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // FK to the User placing the order
        public User User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // e.g., Pending, Completed, Cancelled
        public string PaymentStatus { get; set; } = "Pending"; // e.g., Pending, Paid, Refunded

        public string? PaymentIntentId { get; set; } // Stripe Payment Intent ID

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
