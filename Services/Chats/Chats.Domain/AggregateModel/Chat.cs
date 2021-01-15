using Chats.Domain.Exceptions;
using Chats.Domain.SeedWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public class Chat : Entity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string OwnerId { get; private set; }
        public User Owner { get; private set; }

        public int Capacity { get; private set; }

        private readonly List<ChatModerator> _chatModerators = new List<ChatModerator>();
        public IReadOnlyCollection<ChatModerator> ChatModerators => _chatModerators;
        public IReadOnlyCollection<User> Moderators => ChatModerators
            .Where(cm => !cm.IsTransient)
            .Select(cm => cm.User)
            .ToList();

        private readonly List<ChatMembership> _chatMemberships = new List<ChatMembership>();
        public IReadOnlyCollection<ChatMembership> ChatMemberships => _chatMemberships;
        public IReadOnlyCollection<User> Members => ChatMemberships
            .Where(cm => !cm.IsTransient)
            .Select(cm => cm.User)
            .ToList();

        private readonly List<Message> _messages = new List<Message>();
        public IReadOnlyCollection<Message> Messages => _messages;

        private Chat() { }

        public Chat(string name, User owner, int capacity)
        {
            Name = name;
            Owner = owner;

            if (capacity < 2)
            {
                throw new ChatDomainException($"The minimum chat capacity is 2");
            }
            Capacity = capacity;
        }

        public void AddModerator(string userId)
        {
            if (IsPrivate())
            {
                throw new ChatDomainException($"Chat {this.Id} is a private chat");
            }

            if (HasModerator(userId))
            {
                throw new ChatDomainException($"User with id {userId} is already a moderator of chat {this.Id}");
            }

            if (!HasMember(userId))
            {
                throw new ChatDomainException($"User {userId} is not a member of chat {this.Id}");
            }

            var chatModerator = new ChatModerator(Members.First(user => user.Id == userId));
            _chatModerators.Add(chatModerator);
        }

        public void AddMember(User user)
        {
            if (_chatMemberships.Count == Capacity)
            {
                throw new ChatDomainException($"Chat {this.Id} is full");
            }

            if (HasMember(user.Id))
            {
                throw new ChatDomainException($"User with id {user.Id} is already a member of chat {this.Id}");
            }

            var chatMembership = new ChatMembership(user);
            _chatMemberships.Add(chatMembership);
        }

        public void AddMessage(string content, string authorId)
        {
            _messages.Add(new Message(content, authorId));
        }

        public bool IsPrivate()
        {
            return Capacity == 2;
        }

        public bool HasMember(string userId) 
            => Members.Any(user => user.Id == userId);

        public bool HasModerator(string userId) 
            => Moderators.Any(user => user.Id == userId);
    }
}
