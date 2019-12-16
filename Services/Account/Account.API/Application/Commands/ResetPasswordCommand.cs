using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Account.API.Application.Commands
{
    public class ResetPasswordCommand : INotification
    {
        public string UserName { get; set; }

        [Required]
        public string CompletionUrl { get; set; }
    }
}
