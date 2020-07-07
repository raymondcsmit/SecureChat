using System;
using System.Collections.Generic;

namespace Chats.Domain.Exceptions
{
    public class ChatDomainException : Exception
    {
        public IEnumerable<string> Errors { get; } = new List<string>();

        public ChatDomainException()
        { }

        public ChatDomainException(string message, IEnumerable<string> errors = null)
            : base(message)
        {
            Errors = errors ?? new[] { message };
        }

        public ChatDomainException(string message, Exception innerException, IEnumerable<string> errors = null)
            : base(message, innerException)
        {
            Errors = errors ?? new[] { message };
        }
    }
}
