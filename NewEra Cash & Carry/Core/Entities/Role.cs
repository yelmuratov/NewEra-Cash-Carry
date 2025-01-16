using NewEra_Cash___Carry.Core.Entities;
using System.ComponentModel.DataAnnotations;

public class Role
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
