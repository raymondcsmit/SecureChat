using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Application.Commands
{
    public class UpdateFriendshipRequestStatusCommand : INotification
    {
        public string Id { get; set; }

        [Required, RegularExpression("accepted|rejected")]
        public string Outcome { get; set; }
    }
}
