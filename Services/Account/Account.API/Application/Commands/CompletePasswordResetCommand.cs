using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Account.API.Application.Commands
{
    public class CompletePasswordResetCommand: INotification
    {
        [Required]
        public string Token { get; set; }

        public string Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
