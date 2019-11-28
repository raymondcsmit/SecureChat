using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Users.API.Application.Commands
{
    public class ResetPasswordCommand : INotification
    {
        public string UserName { get; set; }

        [Required]
        public string CompletionUrl { get; set; }
    }
}
