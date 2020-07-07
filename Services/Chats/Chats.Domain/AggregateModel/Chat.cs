using Chats.Domain.Exceptions;
using Chats.Domain.SeedWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Chats.Domain.AggregateModel
{
    public class Chat : Entity, IAggregateRoot
    {
        public string Name { get; private set; }

        public string OwnerId { get; private set; }

        private readonly HashSet<ChatModerator> _chatModerators = new HashSet<ChatModerator>(new ChatModeratorEqualityComparer());
        public IReadOnlyCollection<ChatModerator> ChatModerators => _chatModerators;

        private readonly HashSet<ChatMembership> _chatMemberships = new HashSet<ChatMembership>(new ChatMembershipEqualityComparer());
        public IReadOnlyCollection<ChatMembership> ChatMemberships => _chatMemberships;

        private readonly List<Message> _messages = new List<Message>();
        public IReadOnlyCollection<Message> Messages => _messages;

        public Chat(string name, string ownerId)
        {
            Name = name;
            OwnerId = ownerId;
        }

        public void AddModerator(string userId)
        {
            var chatModerator = new ChatModerator(userId) {ChatId = this.Id};
            if (_chatModerators.Contains(chatModerator))
            {
                throw new ChatDomainException($"User with id {userId} is already a moderator of chat {this.Id}");
            }

            _chatModerators.Add(chatModerator);
        }

        public void AddMember(string userId)
        {
            var chatMembership = new ChatMembership(userId) {ChatId = this.Id};
            if (_chatMemberships.Contains(chatMembership))
            {
                throw new ChatDomainException($"User with id {userId} is already a member of chat {this.Id}");
            }

            _chatMemberships.Add(chatMembership);
        }

        public void AddMessage(string content, string authorId)
        {
            _messages.Add(new Message(content, authorId));
        }

        private class ChatMembershipEqualityComparer : IEqualityComparer<ChatMembership>
        {
            public bool Equals(ChatMembership x, ChatMembership y)
            {
                return x?.GetHashCode() == y?.GetHashCode();
            }

            public int GetHashCode(ChatMembership obj)
            {
                return obj.ChatId.GetHashCode() ^ obj.UserId.GetHashCode();
            }
        }

        private class ChatModeratorEqualityComparer : IEqualityComparer<ChatModerator>
        {
            public bool Equals(ChatModerator x, ChatModerator y)
            {
                return x?.GetHashCode() == y?.GetHashCode();
            }

            public int GetHashCode(ChatModerator obj)
            {
                return obj.ChatId.GetHashCode() ^ obj.UserId.GetHashCode();
            }
        }
    }
}
