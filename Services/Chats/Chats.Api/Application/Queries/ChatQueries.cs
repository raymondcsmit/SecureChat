using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chats.Api.Dtos;
using Chats.Domain.AggregateModel;
using Chats.Domain.SeedWork;
using Chats.Domain.Specification;
using Chats.Infrastructure;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Chats.Api.Application.Queries
{
    public class ChatQueries : IChatQueries
    {
        private readonly IGenericRepository<Chat> _genericRepository;
        private readonly IMapper _mapper;

        public ChatQueries(IGenericRepository<Chat> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<ChatDto> GetChatById(string id)
        {
            var chat = await _genericRepository.GetAsync(id);
            return chat == null
                ? null
                : _mapper.Map<ChatDto>(chat);
        }

        public async Task<(IEnumerable<ChatDto>, int)> GetChatsForOwnerOrMemberAsync(ISpecification<Chat> spec, string userId)
        {
            spec.Criteria.Add(chat => chat.OwnerId == userId || chat.ChatMemberships.Any(membership => membership.UserId == userId));
            return await GetChats(spec);
        }

        public async Task<(IEnumerable<ChatDto>, int)> GetChats(ISpecification<Chat> spec)
        {
            var (result, total) = await _genericRepository.GetAsync(spec);
            var dtos = _mapper.Map<IEnumerable<ChatDto>>(result);
            return (dtos, total);
        }
    }
}
