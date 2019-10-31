using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class PasswordResetCompletionPostDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
