using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chats.Api.Dtos;
using Chats.Domain.AggregateModel;

namespace Chats.Api.Mapping
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<Chat, ChatDto>();
        }
    }
}
