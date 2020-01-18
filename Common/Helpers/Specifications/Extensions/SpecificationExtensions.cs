using Helpers.Specifications.Attributes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Helpers.Specifications.Extensions
{
    public static class SpecificationExtensions
    {
        public static (string, object) Apply<T>(this ISpecification<T> spec, string baseQuery)
        {
            var (query, preparedStatementObj) = ApplyCriteria(spec.Criteria, baseQuery);
            query = ApplyOrderBy(spec.OrderBy, query);
            if (spec.IsPagingEnabled)
            {
                query = ApplyPaging(spec.Limit, spec.Offset, query);
            }

            return (query, preparedStatementObj);
        }

        private static (string, object) ApplyCriteria(IReadOnlyCollection<Criteria> criteria, string baseQuery)
        {
            var preparedStatementObject = new ExpandoObject() as IDictionary<string, object>;
            if (!criteria.Any())
            {
                return (baseQuery, preparedStatementObject);
            }

            var clauses = new List<string>();
            foreach (var c in criteria)
            {
                var currentCount = preparedStatementObject.Count;
                clauses.Add($@"{c.TableName}.{c.ColumnName} = @val{currentCount}");
                preparedStatementObject[c.ColumnName] = c.Value;
            }

            return ($@"{baseQuery.Trim(';')} 
                       WHERE{string.Join(" AND ", clauses)};", preparedStatementObject);
        }

        private static string ApplyOrderBy(IReadOnlyCollection<OrderByColumn> columns, string baseQuery)
        {
            if (!columns.Any())
            {
                return baseQuery;
            }

            var cols = columns
                .Select(col => $"{col.ColumnName} {col.Mode}");
            var orderByStr = string.Join(",", cols);

            return ($@"{baseQuery.Trim(';')} 
                        ORDER BY {orderByStr};");
        }

        private static string ApplyPaging(int limit, int offset, string baseQuery)
        {
            return $@"{baseQuery.Trim(';')} 
                  LIMIT {limit} OFFSET {offset};";
        }
    }
}
