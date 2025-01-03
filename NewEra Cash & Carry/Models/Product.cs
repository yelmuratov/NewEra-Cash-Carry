using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Models
{
        public class Product
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int ProductId { get; set; }

            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [StringLength(500)]
            public string Description { get; set; }

            [Column(TypeName = "decimal(18, 2)")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
            public decimal Price { get; set; }


            [Required]
            [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative value.")]
            public int Stock { get; set; }

            [Required]
            [ForeignKey("Category")]
            public int CategoryId { get; set; }

            public Category Category { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? UpdatedAt { get; set; }
        }
    }
