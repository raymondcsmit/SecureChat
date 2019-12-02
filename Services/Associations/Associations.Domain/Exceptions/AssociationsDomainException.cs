using System;
using System.Collections.Generic;

namespace Associations.Domain.Exceptions
{
    public class AssociationsDomainException : Exception
    {
        public IEnumerable<string> Errors { get; } = new List<string>();

        public AssociationsDomainException()
        { }

        public AssociationsDomainException(string message, IEnumerable<string> errors = null)
            : base(message)
        {
            if (errors != null) Errors = errors;
        }

        public AssociationsDomainException(string message, Exception innerException, IEnumerable<string> errors = null)
            : base(message, innerException)
        {
            if (errors != null) Errors = errors;
        }
    }
}
