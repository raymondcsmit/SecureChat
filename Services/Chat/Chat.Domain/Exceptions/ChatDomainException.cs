using System;
using System.Collections.Generic;

namespace Chat.Domain.Exceptions
{
    public class ChatDomainException : Exception
    {
        public IEnumerable<string> Errors { get; } = new List<string>();

        public ChatDomainException()
        { }

        public ChatDomainException(string message, IEnumerable<string> errors = null)
            : base(message)
        {
            if (errors != null) Errors = errors;
        }

        public ChatDomainException(string message, Exception innerException, IEnumerable<string> errors = null)
            : base(message, innerException)
        {
            if (errors != null) Errors = errors;
        }
    }
}
