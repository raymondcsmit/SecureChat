using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Users.API.Application.Commands
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
