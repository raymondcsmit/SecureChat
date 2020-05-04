using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Session.API.Dto;
using Session.API.Infrastructure;
using Session.API.Infrastructure.Exceptions;
using Session.API.Model;

namespace Session.API.Services
{
    public class SessionService : ISessionService
    {
        private readonly SessionContext _context;

        public SessionService(SessionContext context)
        {
            _context = context;
        }

        public async Task<SessionDto> CreateSessionAsync(string userId)
        {
            var user = _context.ChatUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _context.Add(new ChatUser { Id = userId });
            }
            else
            {
                _context.Entry(user).State = EntityState.Modified;
            }

            var chatEvent = new ChatEvent { EventType = EventType.Connected };
            _context.ChatEvents.Add(chatEvent);

            _context.ChatUserEvents.Add(new ChatUserEvent { User = user, Event = chatEvent });

            await _context.SaveChangesAsync();

            return new SessionDto
            {
                StartTime = chatEvent.CreatedAt,
                IdleTime = TimeSpan.Zero,
                PreviousEndTime = DateTimeOffset.MinValue
            };
        }

        public async Task<SessionDto> GetSessionAsync(string userId)
        {
            var user = await _context.ChatUsers
                .Include(u => u.EventsLink)
                .ThenInclude(ue => ue.Event)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw SessionApiException.NotFound;
            }

            var lastConnectedTime = user.EventsLink
                .Select(ue => ue.Event)
                .Where(e => e.EventType == EventType.Connected)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault()?.CreatedAt ?? throw SessionApiException.NotFound;

            var idleTime = DateTimeOffset.Now - user.ModifiedAt;

            var lastDisconnectedTime = user.EventsLink
                .Select(ue => ue.Event)
                .Where(e => e.EventType == EventType.Disconnected)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault()?.CreatedAt ?? DateTimeOffset.MinValue;

            if (lastConnectedTime < lastDisconnectedTime)
            {
                throw SessionApiException.NotFound;
            }

            return new SessionDto
            {
                StartTime = lastConnectedTime,
                IdleTime = idleTime,
                PreviousEndTime = lastDisconnectedTime
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
    }
}
