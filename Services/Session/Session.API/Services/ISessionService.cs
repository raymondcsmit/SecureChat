using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Session.API.Dto;

namespace Session.API.Services
{
    public interface ISessionService
    {
        Task<SessionDto> CreateSessionAsync(string userId);
        Task<SessionDto> GetSessionAsync(string userId);
        Task RefreshSessionAsync(string userId);

        Task EndSessionAsync(string userId);
    }
}
