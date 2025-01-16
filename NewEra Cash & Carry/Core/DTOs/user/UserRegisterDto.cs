using System.ComponentModel.DataAnnotations;

namespace NewEra_Cash___Carry.Core.DTOs.user
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(15)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
    }
}

