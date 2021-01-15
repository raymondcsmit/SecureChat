using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chats.Domain.AggregateModel;
using Chats.Domain.Exceptions;
using Chats.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Chats.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatsContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public ChatRepository(ChatsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Chat Create(Chat chat) 
            => _context.Chats.Add(chat).Entity;

        public async Task<Chat> GetAsync(string id) =>
            await _context
                .Chats
                .Include(c => c.Messages)
                    .ThenInclude(m => m.Chat)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.User)
                .Include(c => c.ChatMemberships)
                    .ThenInclude(c => c.Chat)
                .Include(c => c.ChatMemberships)
                    .ThenInclude(c => c.User)
                .Include(c => c.ChatModerators)
                    .ThenInclude(c => c.Chat)
                .Include(c => c.ChatModerators)
                    .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(c => c.Id == id);

        public async Task DeleteAsync(string id)
        {
            var chat = await _context.Chats.FindAsync(id) 
                       ?? throw new ChatDomainException("Could not delete the chat", new[] {"Chat not found"});

            _context.Chats.Remove(chat);
        }

        public Chat Update(Chat chat) 
            => _context.Chats.Update(chat).Entity;
    }
}
