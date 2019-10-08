using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Infrastructure.Exceptions
{
    public class UsersApiException : Exception
    {
        public IEnumerable<string> Errors { get; } = new List<string>();

        public int ErrorCode { get; }

        public UsersApiException()
        { }

        public UsersApiException(string message, IEnumerable<string> errors = null, int errorCode = 400)
            : base(message)
        {
            ErrorCode = errorCode;
            if (errors != null) Errors = errors;
        }

        public UsersApiException(string message, Exception innerException, IEnumerable<string> errors = null, int errorCode = 400)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            if (errors != null) Errors = errors;
        }
    }
}
