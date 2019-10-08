using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class PasswordResetCompletionGetDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string UserName { get; set; }

        public string LoginUrl { get; set; }
    }
}
