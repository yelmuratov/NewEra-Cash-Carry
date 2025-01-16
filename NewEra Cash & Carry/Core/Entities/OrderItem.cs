using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; } // FK to the Order
        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; } // FK to the Product
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
