using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Chats.Domain.Specification;
using Microsoft.EntityFrameworkCore;

namespace Chats.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyCriteria<T>(this IQueryable<T> queryable, ISpecification<T> spec)
        {
            var result = spec.Criteria.Aggregate(queryable, 
                (current, criterion) => current.Where(criterion));

            foreach (var criterion in spec.StringCriteria)
            {
                var property = typeof(T).GetProperty(criterion.Property);
                var parameter = Expression.Parameter(typeof(T), "obj");
                var getter = Expression.Property(parameter, property.Name);
                var value = Expression.Constant(criterion.Value);

                var comparator = criterion.Mode switch
                {
                    ComparisonMode.GreaterThan => Expression.GreaterThan(getter, value),
                    ComparisonMode.LessThan => Expression.LessThan(getter, value),
                    _ => Expression.Equal(getter, value)
                };

                result = result.Where(Expression.Lambda<Func<T, bool>>(comparator, parameter));
            }

            return result;
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, ISpecification<T> spec)
            => spec.IsPaginationEnabled ? queryable.Skip(spec.Offset).Take(spec.Limit) : queryable;

        public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> queryable, ISpecification<T> spec) where T : class
        {
            var result = spec.Includes
                .Aggregate(queryable,
                    (current, include) => current.Include(include));

            Expression<Func<T1, T2>> MakePropertyGetterExperssion<T1, T2>(PropertyInfo property)
            {
                var parameter = Expression.Parameter(typeof(T), "obj");
                var getter = Expression.Property(parameter, property.Name);
                return Expression.Lambda<Func<T1, T2>>(getter, parameter);
            }

            foreach (var include in spec.StringIncludes)
            {
                var property = typeof(T).GetProperty(include.Include);
                if (property == null)
                {
                    throw new InvalidOperationException($"{include.Include} is not a valid include property");
                }

                PropertyInfo thenIncludeProperty = null;
                if (include.ThenInclude != null)
                {
                    thenIncludeProperty = property.PropertyType.GetProperty(include.ThenInclude);
                    if (thenIncludeProperty == null)
                    {
                        throw new InvalidOperationException($"{include.ThenInclude} is not a valid then include property");
                    }
                }

                var result2 = result.Include(MakePropertyGetterExperssion<T, object>(property));
                if (thenIncludeProperty != null)
                {
                    result2.ThenInclude(MakePropertyGetterExperssion<object, object>(thenIncludeProperty));
                }

                return result2;
            }

            return result;
        }

        public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> queryable, ISpecification<T> spec) where T : class
        {
            var orderBy = spec.StringOrderBy.FirstOrDefault();
            if (orderBy == null)
            {
                return queryable;
            }
            var thenBy = spec.StringOrderBy.Skip(1);

            var property = typeof(T).GetProperty(orderBy.Key);
            if (property == null)
            {
                throw new InvalidOperationException($"{orderBy.Key} is not a valid property");
            }

            var expressionParameter = Expression.Parameter(typeof(T), "obj");
            var expressionProperty = Expression.Property(expressionParameter, property.Name);
            var lambda = Expression.Lambda<Func<T, object>>(expressionProperty, expressionParameter);

            var result = orderBy.Mode == OrderByMode.Ascending
                ? queryable.OrderBy(lambda)
                : queryable.OrderByDescending(lambda);

            return thenBy.Aggregate(result, (currentResult, nextOrderBy) =>
            {
                var property = typeof(T).GetProperty(nextOrderBy.Key);
                if (property == null)
                {
                    throw new InvalidOperationException($"{orderBy.Key} is not a valid property");
                }

                var expressionProperty = Expression.Property(expressionParameter, property.Name);
                var lambda = Expression.Lambda<Func<T, object>>(expressionProperty, expressionParameter);
                return nextOrderBy.Mode == OrderByMode.Ascending
                                    ? currentResult.ThenBy(lambda)
                                    : currentResult.ThenByDescending(lambda);
            });
        }
    }
}
