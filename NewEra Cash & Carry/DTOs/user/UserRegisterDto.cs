using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.DTOs.user
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
