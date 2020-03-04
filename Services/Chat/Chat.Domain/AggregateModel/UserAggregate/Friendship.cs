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

        public string FirstFriendId { get; private set; }

        public string SecondFriendId { get; private set; }

        public Friendship(string firstFriendId, string secondFriendId)
        {
            if (firstFriendId == secondFriendId)
            {
                throw new ChatDomainException("Friend ids cannot be equal");
            }

            FirstFriendId = firstFriendId;
            SecondFriendId = secondFriendId;


            CreatedAt = DateTimeOffset.Now;
            ModifiedAt = DateTimeOffset.Now;
        }
    }
}
