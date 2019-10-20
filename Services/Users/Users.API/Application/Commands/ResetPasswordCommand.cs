using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
