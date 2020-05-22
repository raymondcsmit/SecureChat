using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Session.API.Dto;

namespace Session.API.Services
{
    public interface ISessionService
    {
        Task<ChatSessionDto> CreateSessionAsync(string userId);
        Task<ChatSessionDto> GetSessionAsync(string userId);
        Task RefreshSessionAsync(string userId);
        Task EndSessionAsync(string userId);
        Task<IDictionary<string, ChatSessionDto>> GetChatSessionsForFriendsById(string userId);
    }
}
