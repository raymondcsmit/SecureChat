using System.ComponentModel.DataAnnotations;

namespace Registration.ViewModels
{
    public class ResetPasswordGetViewModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
