using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Users.API.Application.Commands
{
    public class ConfirmEmailCommand : INotification
    {
        [Required]
        public string Id { get; private set; }

        [Required]
        public string Token { get; private set; }
    }
}
