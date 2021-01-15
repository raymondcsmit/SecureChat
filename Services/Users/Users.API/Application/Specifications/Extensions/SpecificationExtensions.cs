using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Users.API.Application.Specifications.Extensions
{
    public static class SpecificationExtensions
    {
        public static (string, object) Apply<T>(this ISpecification<T> spec, string baseQuery, bool withPagination = true, IDictionary<string, object> additionalProps = null)
        {
            var (query, preparedStatementObj) = ApplyCriteria(spec.Criteria, spec.ExcludeCriteria, baseQuery);
            query = ApplyOrderBy(spec.OrderBy, query);
            
            if (spec.IsPagingEnabled && withPagination)
            {
                query = ApplyPaging(spec.Limit, spec.Offset, query);
            }

            preparedStatementObj = additionalProps?.Union(preparedStatementObj)
                ?.GroupBy(x => x.Key)
                ?.ToDictionary(g => g.Key,
                    g => g.First().Value) ?? preparedStatementObj;

            return (query, preparedStatementObj);
        }

        private static (string, IDictionary<string, object>) ApplyCriteria(IReadOnlyCollection<Criteria> criteria, IReadOnlyCollection<Criteria> excludeCriteria, string baseQuery)
        {
            var preparedStatementObject = new ExpandoObject() as IDictionary<string, object>;
            if (!criteria.Any())
            {
                return (baseQuery, preparedStatementObject);
            }

            var clauses = new List<string>();
            foreach (var c in criteria)
            {
                var currentCount = preparedStatementObject.Count - 1;
                clauses.Add($@"{c.TableName}.{c.ColumnName} = @val{currentCount + 1}");
                preparedStatementObject[$"val{currentCount + 1}"] = c.Value;
            }
            foreach (var c in excludeCriteria)
            {
                var currentCount = preparedStatementObject.Count - 1;
                clauses.Add($@"{c.TableName}.{c.ColumnName} != @val{currentCount + 1}");
                preparedStatementObject[$"val{currentCount + 1}"] = c.Value;
            }

            return ($@"{baseQuery.Trim(';')} 
                       WHERE {string.Join(" AND ", clauses)};", preparedStatementObject);
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
