using System.ComponentModel.DataAnnotations;

namespace Registration.Models
{
    public class EmailConfirmationDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
