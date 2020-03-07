using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class Friendship: Entity, IAuditable
    {
        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset ModifiedAt { get; private set; }

        public string UserId1 { get; private set; }

        public string UserId2 { get; private set; }

        public Friendship(string userId1, string userId2)
        {
            if (userId1 == userId2)
            {
                throw new ChatDomainException("Friend ids cannot be equal");
            }

            UserId1 = userId1;
            UserId2 = userId2;
        }
    }
}
