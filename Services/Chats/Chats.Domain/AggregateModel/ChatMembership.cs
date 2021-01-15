using Chats.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public sealed class ChatMembership: Entity, IAuditable
    {
        public string ChatId { get; private set; }
        public Chat Chat { get; private set; }

        public string UserId { get; private set; }
        public User User { get; private set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        private ChatMembership() { }

        public ChatMembership(User user)
        {
            User = user;
            UserId = user.Id;
        }
    }
}
