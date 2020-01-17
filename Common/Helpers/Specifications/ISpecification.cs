using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications
{
    public interface ISpecification<T>
    {
        object PreparedStatementObject { get; }
        string Criteria { get; }
        string OrderBy { get; }
        string Pagination { get; }
        bool IsPagingEnabled { get; }
    }
}
