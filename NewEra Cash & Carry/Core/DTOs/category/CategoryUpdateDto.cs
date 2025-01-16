using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.DTOs.category
{
    public class CategoryUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
