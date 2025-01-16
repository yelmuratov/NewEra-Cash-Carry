using NewEra_Cash___Carry.Core.Entities;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(15)]
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
}
