using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class PasswordResetDto
    {
        [Required]
        public string UserName { get; set; }

        public string LoginUrl { get; set; }
    }
}
