using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.DTOs.order
{
    public class OrderItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
