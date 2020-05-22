using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Session.API.Dto;
using Session.API.Infrastructure;
using Session.API.Infrastructure.Exceptions;
using Session.API.Model;
using Users.API.Client;

namespace Session.API.Services
{
    public class SessionService : ISessionService
    {
        private readonly SessionContext _context;
        private readonly IUsersApiClient _usersApiClient;

        public SessionService(SessionContext context, IUsersApiClient usersApiClient)
        {
            _context = context;
            _usersApiClient = usersApiClient;
        }

        public async Task<ChatSessionDto> CreateSessionAsync(string userId)
        {
            var user = _context.ChatUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                user = new ChatUser {Id = userId};
                _context.Add(user);
            }
            else
            {
                _context.Entry(user).State = EntityState.Modified;
            }

            var chatEvent = new ChatEvent { EventType = EventType.Connected };
            _context.ChatEvents.Add(chatEvent);

            _context.ChatUserEvents.Add(new ChatUserEvent { User = user, Event = chatEvent });

            await _context.SaveChangesAsync();

            return new ChatSessionDto
            {
                StartTime = chatEvent.CreatedAt,
                IdleTime = TimeSpan.Zero,
                EndTime = DateTimeOffset.MinValue
            };
        }

        public async Task<ChatSessionDto> GetSessionAsync(string userId)
        {
            var user = await _context.ChatUsers
                .Include(u => u.EventsLink)
                .ThenInclude(ue => ue.Event)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw SessionApiException.NotFound;
            }

            var startTime = user.EventsLink
                .Select(ue => ue.Event)
                .Where(e => e.EventType == EventType.Connected)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault()?.CreatedAt ?? throw SessionApiException.NotFound;

            var idleTime = DateTimeOffset.Now - user.ModifiedAt;

            var endTime = user.EventsLink
                .Select(ue => ue.Event)
                .Where(e => e.EventType == EventType.Disconnected)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault()?.CreatedAt ?? DateTimeOffset.MinValue;

            if (startTime < endTime)
            {
                throw SessionApiException.NotFound;
            }

            return new ChatSessionDto
            {
                StartTime = startTime,
                IdleTime = idleTime,
                EndTime = endTime
            };
        }

        public async Task RefreshSessionAsync(string userId)
        {
            var user = _context.ChatUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw SessionApiException.NotFound;
            }

            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task EndSessionAsync(string userId)
        {
            var user = _context.ChatUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw SessionApiException.NotFound;
            }

            _context.Entry(user).State = EntityState.Modified;

            var chatEvent = new ChatEvent { EventType = EventType.Disconnected };
            _context.ChatEvents.Add(chatEvent);

            _context.ChatUserEvents.Add(new ChatUserEvent { User = user, Event = chatEvent });

            await _context.SaveChangesAsync();
        }

        public async Task<IDictionary<string, ChatSessionDto>> GetChatSessionsForFriendsById(string userId)
        {
            var friendships = await _usersApiClient.GetFriendshipsByUserId(userId);
            var sessions = new Dictionary<string, ChatSessionDto>();
            foreach (var friendship in friendships)
            {
                var friendId = friendship.User1.Id != userId ? friendship.User1.Id : friendship.User2.Id;
                try
                {
                    var session = await GetSessionAsync(friendId);
                    sessions[friendId] = session;
                }
                catch (SessionApiException e)
                {}
            }

            return sessions;
        }
    }
}
