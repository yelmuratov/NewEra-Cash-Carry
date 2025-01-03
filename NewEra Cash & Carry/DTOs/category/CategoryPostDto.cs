using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.DTOs.category
{
    public class CategoryPostDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
