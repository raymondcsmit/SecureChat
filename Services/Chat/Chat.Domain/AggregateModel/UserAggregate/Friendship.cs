﻿using System;
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

        public string User1Id { get; private set; }

        public string User2Id { get; private set; }

        public Friendship() { }

        public Friendship(string user1Id, string user2Id)
        {
            if (user1Id == user2Id)
            {
                throw new ChatDomainException("Friend ids cannot be equal");
            }

            User1Id = user1Id;
            User2Id = user2Id;
        }
    }
}
