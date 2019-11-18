using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Application.Commands
{
    public class ConfirmEmailCommand : INotification
    {
        public string Id { get; set; }

        public string Token { get; set; }
    }
}
