using Chats.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public class Message: Entity, IAuditable
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public string Content { get; private set; }

        public string UserId { get; private set; }
        public User User { get; private set; }

        public string ChatId { get; private set; }
        public Chat Chat { get; private set; }

        private Message() { }

        public Message(string content, string userId)
        {
            Content = content;
            UserId = userId;
        }
    }
}
