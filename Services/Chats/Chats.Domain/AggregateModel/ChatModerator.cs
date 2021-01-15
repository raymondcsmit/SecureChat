using Chats.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public sealed class ChatModerator: Entity
    {
        public string ChatId { get; private set; }
        public Chat Chat { get; private set; }

        public string UserId { get; private set; }
        public User User { get; private set; }

        private ChatModerator() { }

        public ChatModerator(User user)
        {
            User = user;
            UserId = user.Id;
        }
    }
}
