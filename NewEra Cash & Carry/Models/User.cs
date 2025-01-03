using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        // Navigation property for roles
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
