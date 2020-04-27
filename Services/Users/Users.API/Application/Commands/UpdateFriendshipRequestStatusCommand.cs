using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Users.API.Application.Commands
{
    public class UpdateFriendshipRequestStatusCommand : INotification
    {
        public string Id { get; set; }

        [Required, RegularExpression("accepted|rejected")]
        public string Outcome { get; set; }
    }
}
