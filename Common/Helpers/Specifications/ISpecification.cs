using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications
{
    public interface ISpecification<T>
    {
        IReadOnlyCollection<Criteria> Criteria { get; }
        IReadOnlyCollection<OrderByColumn> OrderBy { get; }
        bool IsPagingEnabled { get; }
        int Limit { get; }
        int Offset { get; }
    }
}
