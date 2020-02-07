using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Dtos;
using MediatR;

namespace Chat.API.Application.Commands
{
    public class MakeFriendshipRequestCommand : IRequest<FriendshipRequestDto>
    {
        [Required]
        public string RequesterId { get; }

        [Required]
        public string RequesteeId { get; }
    }
}
