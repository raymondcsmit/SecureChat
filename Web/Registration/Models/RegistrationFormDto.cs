using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Registration.Models
{
    public class RegistrationFormDto
    {
        [Required]
        public string UserName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password), Compare(nameof(Password))]
        [JsonIgnore]
        public string ConfirmPassword { get; set; }
    }
}
