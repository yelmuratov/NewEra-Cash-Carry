using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.DTOs.order
{
    public class OrderCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<OrderItemCreateDto> OrderItems { get; set; }
    }
}
