namespace NewEra_Cash___Carry.Core.DTOs.role;

using System.ComponentModel.DataAnnotations;

public class RoleDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}
