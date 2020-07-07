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

        public string Content { get; }

        public string UserId { get; }

        public string ChatId { get; set; }
        public Chat Chat { get; set; }

        public Message(string content, string userId)
        {
            Content = content;
            UserId = userId;
        }
    }
}
