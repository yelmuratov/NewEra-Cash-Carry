using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.DTOs.product
{
    public class ProductPostDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative value.")]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
