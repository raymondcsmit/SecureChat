using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class PasswordResetCompletionPostDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
