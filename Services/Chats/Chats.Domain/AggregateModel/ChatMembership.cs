using Chats.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public sealed class ChatMembership: Entity, IAuditable
    {
        public string ChatId { get; set; }
        public Chat Chat { get; set; }

        public string UserId { get; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public ChatMembership(string userId)
        {
            UserId = userId;
        }
    }
}
