using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications
{
    public interface ISpecification<T>
    {
        dynamic PreparedStatementObject { get; }
        bool IsPagingEnabled { get; }
        string Apply(string baseQuery);
    }
}
