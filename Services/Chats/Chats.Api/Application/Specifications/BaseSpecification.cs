using Chats.Domain.Specification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chats.Api.Application.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public List<Expression<Func<T, bool>>> Criteria { get; } = new List<Expression<Func<T, bool>>>();

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public List<StringInclude> StringIncludes { get; } = new List<StringInclude>();

        public List<StringCriterion> StringCriteria { get; } = new List<StringCriterion>();

        public List<StringOrderBy> StringOrderBy { get; } = new List<StringOrderBy>();

        public int Limit { get; private set; }

        public int Offset { get; private set; }

        public bool IsPaginationEnabled => Limit != default;

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(StringInclude include)
        {
            StringIncludes.Add(include);
        }

        protected virtual void AddCriteria(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria.Add(criteriaExpression);
        }
        
        protected virtual void AddCriterion(StringCriterion stringCriterion)
        {
            StringCriteria.Add(stringCriterion);
        }

        protected virtual void AddCriteria(IEnumerable<StringCriterion> stringCriteria)
        {
            StringCriteria.AddRange(stringCriteria);
        }

        protected virtual void AddOrderBy(StringOrderBy stringOrderBy)
        {
            StringOrderBy.Add(stringOrderBy);
        }

        protected virtual void AddOrderBy(IEnumerable<StringOrderBy> stringOrderBy)
        {
            StringOrderBy.AddRange(stringOrderBy);
        }

        protected virtual void AddPagination(int limit, int offset)
        {
            Limit = limit;
            Offset = offset;
        }
    }
}
