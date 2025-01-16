using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public string ImageUrl { get; set; }
    }
}
