using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Chat.API.Infrastructure.Exceptions
{
    public class AssociationsApiException : Exception
    {
        public IEnumerable<string> Errors { get; } = new List<string>();

        public int ErrorCode { get; }

        public AssociationsApiException()
        { }

        public AssociationsApiException(string message, IEnumerable<string> errors = null, int errorCode = 400)
            : base(message)
        {
            ErrorCode = errorCode;
            if (errors != null) Errors = errors;
        }

        public AssociationsApiException(string message, Exception innerException, IEnumerable<string> errors = null, int errorCode = 400)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            if (errors != null) Errors = errors;
        }

        public AssociationsApiException(string message, IEnumerable<IdentityError> errors, int errorCode = 400)
            : base(message)
        {
            ErrorCode = errorCode;
            if (errors != null) Errors = errors.Select(e => e.Description);
        }
    }
}
