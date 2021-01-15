using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chats.Domain.Specification
{
    public interface ISpecification<T>
    {
        List<Expression<Func<T, bool>>> Criteria { get; }
        List<StringCriterion> StringCriteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<StringInclude> StringIncludes { get; }
        List<StringOrderBy> StringOrderBy { get; }
        int Limit { get; }
        int Offset { get; }
        bool IsPaginationEnabled { get; }
    }
}
