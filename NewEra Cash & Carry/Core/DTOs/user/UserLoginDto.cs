using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.DTOs.user
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
