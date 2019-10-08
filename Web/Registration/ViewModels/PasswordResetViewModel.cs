using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Registration.ViewModels
{
    public class PasswordResetViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password), Compare(nameof(Password))]
        [JsonIgnore]
        public string ConfirmPassword { get; set; }
    }
}
