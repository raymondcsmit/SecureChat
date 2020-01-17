using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Helpers.Specifications.Attributes;

namespace Helpers.Specifications
{
    public class Criteria
    {
        public string ColumnName { get; }
        public string TableName { get; }
        public string Value { get; }

        public Criteria(string columnName, string tableName, string value)
        {
            ColumnName = columnName;
            TableName = tableName;
            Value = value;
        }
    }

    public class OrderByColumn
    {
        public string ColumnName { get; }

        public string Mode { get; }

        public OrderByColumn(string columnName, string mode)
        {
            if (!new[] { OrderByMode.Ascending, OrderByMode.Descending }.Contains(mode))
            {
                throw new ArgumentException();
            }

            ColumnName = columnName;
            Mode = mode;
        }
    }

    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public object PreparedStatementObject { get; } = new ExpandoObject();
        public string Criteria { get; private set; } = "WHERE TRUE";
        public string OrderBy { get; private set; } = string.Empty;
        public string Pagination { get; private set; } = string.Empty;
        public bool IsPagingEnabled => !string.IsNullOrEmpty(Pagination);

        protected virtual void AddCriteria(IEnumerable<Criteria> criteria)
        {
            var sb = new StringBuilder(Criteria);
            var searchableProperties = GetSearchableProperties().ToArray();
            foreach (var c in criteria)
            {
                if (!searchableProperties.Contains(c.ColumnName))
                {
                    continue;
                }
                var preparedStatementObject = PreparedStatementObject as IDictionary<string, object>;
                var currentCount = preparedStatementObject.Count;
                sb.Append($@" AND {c.TableName}.{c.ColumnName} = @val{currentCount}");
                preparedStatementObject[c.ColumnName] = c.Value;
            }

            Criteria = sb.ToString();
        }

        protected virtual void AddOrderBy(IEnumerable<OrderByColumn> columns)
        {
            var sortableProperties = GetSortableProperties().ToArray();
            var validColumns = columns
                .Where(col => sortableProperties.Contains(col.ColumnName))
                .Select(col => $"{col.ColumnName} {col.Mode}");
            var orderByStr = string.Join(",", validColumns);
            if (string.IsNullOrEmpty(orderByStr))
            {
                return;
            }

            OrderBy = string.IsNullOrEmpty(OrderBy) ? $"ORDER BY {orderByStr}" : $"{OrderBy} {orderByStr}";
        }

        protected virtual void AddPaging(int take, int skip) 
            => Pagination = $"LIMIT {take} OFFSET {skip}";

        private IEnumerable<string> GetSearchableProperties() =>
            typeof(T)
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<Searchable>() != null)
                .Select(prop => prop.Name);

        private IEnumerable<string> GetSortableProperties() =>
            typeof(T)
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<Sortable>() != null)
                .Select(prop => prop.Name);
    }
}
