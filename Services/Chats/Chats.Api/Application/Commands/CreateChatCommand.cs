using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chats.Api.Dtos;
using MediatR;

namespace Chats.Api.Application.Commands
{
    public class CreateChatCommand : IRequest<ChatDto>
    {
        [Required]
        public string OwnerId { get; }
        [Required]
        public string Name { get; }
        [Required, Range(2, 1000)]
        public int Capacity { get; }

        public CreateChatCommand(string ownerId, string name, int capacity)
        {
            OwnerId = ownerId;
            Name = name;
            Capacity = capacity;
        }
    }
}
