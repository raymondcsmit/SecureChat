using System.Collections.Generic;

namespace Users.API.Application.Specifications
{
    public interface ISpecification<T>
    {
        IReadOnlyCollection<Criteria> Criteria { get; }
        IReadOnlyCollection<Criteria> ExcludeCriteria { get; }
        IReadOnlyCollection<OrderByColumn> OrderBy { get; }
        bool IsPagingEnabled { get; }
        int Limit { get; }
        int Offset { get; }
    }
}
