using System.Collections.Generic;
using System.Threading.Tasks;
using Chats.Api.Dtos;
using Chats.Domain.AggregateModel;
using Chats.Domain.Specification;

namespace Chats.Api.Application.Queries
{
    public interface IChatQueries
    {
        Task<ChatDto> GetChatById(string id);
        Task<(IEnumerable<ChatDto>, int)> GetChatsForOwnerOrMemberAsync(ISpecification<Chat> spec, string userId);
        Task<(IEnumerable<ChatDto>, int)> GetChats(ISpecification<Chat> spec);
    }
}
