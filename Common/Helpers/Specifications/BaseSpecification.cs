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

    public class BaseSpecification<T> : ISpecification<T>
    {
        public dynamic PreparedStatementObject { get; } = new ExpandoObject();
        private string _criteria;
        private string _orderBy;
        private int _take;
        private int _skip;
        public bool IsPagingEnabled { get; private set; } = false;

        public string Apply(string baseQuery)
        {
            var pagination = IsPagingEnabled ? $"LIMIT {_take} OFFSET {_skip}" : "";
            return $@"{baseQuery} 
                      {_criteria} 
                      {_orderBy}
                      {pagination}";
        }

        protected virtual void AddCriteria(IEnumerable<Criteria> criteria)
        {
            var sb = new StringBuilder("TRUE");
            var searchableProperties = GetSearchableProperties().ToArray();
            foreach (var c in criteria)
            {
                if (!searchableProperties.Contains(c.ColumnName))
                {
                    continue;
                }
                var currentCount = PreparedStatementObject.Count;
                sb.Append($@" AND {c.TableName}.{c.ColumnName} = @val{currentCount}");
                PreparedStatementObject[c.ColumnName] = c.Value;
            }

            _criteria = sb.ToString();
        }

        protected virtual void AddOrderBy(IEnumerable<string> columns, string mode)
        {
            var sortableProperties = GetSortableProperties().ToArray();
            var validColumns = columns.Where(col => sortableProperties.Contains(col));
            var orderByStr = string.Join(",", validColumns.Select(col => $"{col} {mode}"));
            if (string.IsNullOrEmpty(orderByStr))
            {
                return;
            }

            _orderBy = _orderBy == null ? $"ORDER BY {orderByStr}" : $"{_orderBy} {orderByStr}";
        }

        protected virtual void AddPagination(int take, int skip)
        {
            _take = take;
            _skip = skip;
            IsPagingEnabled = true;
        }

        private IEnumerable<string> GetSearchableProperties() =>
            typeof(T)
                .GetProperties(BindingFlags.Public)
                .Where(prop => prop.GetCustomAttribute<Searchable>() != null)
                .Select(prop => prop.Name);

        private IEnumerable<string> GetSortableProperties() =>
            typeof(T)
                .GetProperties(BindingFlags.Public)
                .Where(prop => prop.GetCustomAttribute<Sortable>() != null)
                .Select(prop => prop.Name);
    }
}
