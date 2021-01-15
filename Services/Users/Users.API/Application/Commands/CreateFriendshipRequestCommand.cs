using System.ComponentModel.DataAnnotations;
using Users.API.Dtos;
using MediatR;

namespace Users.API.Application.Commands
{
    public class CreateFriendshipRequestCommand : IRequest<FriendshipRequestDto>
    {
        [Required]
        public string RequesterId { get; }

        [Required]
        public string RequesteeId { get; }

        public CreateFriendshipRequestCommand(string requesterId, string requesteeId)
        {
            RequesterId = requesterId;
            RequesteeId = requesteeId;
        }
    }
}
