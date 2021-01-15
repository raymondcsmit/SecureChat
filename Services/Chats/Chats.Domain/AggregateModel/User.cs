using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chats.Domain.SeedWork;

namespace Chats.Domain.AggregateModel
{
    public class User : Entity
    {
        public string UserName { get; private set; }

        private readonly List<ChatMembership> _chatMemberships = new List<ChatMembership>();
        public IReadOnlyCollection<ChatMembership> ChatMemberships => _chatMemberships;

        public IReadOnlyCollection<Chat> Chats => _chatMemberships
            .Where(cm => !cm.IsTransient)
            .Select(cm => cm.Chat)
            .ToList();

        private User() { }

        public User(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }
    }
}
