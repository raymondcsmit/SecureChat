using Helpers.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chats.Domain.AggregateModel;

namespace Chats.Api.Dtos
{
    public class ChatDto : IMapFrom<Chat>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public UserDto Owner { get; set; }
        public ICollection<UserDto> Members { get; } = new List<UserDto>();
    }
}
