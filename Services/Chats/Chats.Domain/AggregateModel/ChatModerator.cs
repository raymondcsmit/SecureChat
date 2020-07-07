using Chats.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public sealed class ChatModerator: Entity
    {
        public string ChatId { get; set; }
        public Chat Chat { get; set; }

        public string UserId { get; }

        public ChatModerator(string userId)
        {
            UserId = userId;
        }
    }
}
