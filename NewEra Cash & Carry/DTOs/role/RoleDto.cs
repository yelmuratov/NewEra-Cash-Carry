using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.DTOs
{
    public class RoleDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
