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
        private readonly List<Criteria> _criteria = new List<Criteria>();
        public IReadOnlyCollection<Criteria> Criteria => _criteria;

        private readonly List<OrderByColumn> _orderBy = new List<OrderByColumn>();
        public IReadOnlyCollection<OrderByColumn> OrderBy => _orderBy;
        public bool IsPagingEnabled { get; private set; }
        public int Limit { get; private set; }
        public int Offset { get; private set; }

        protected virtual void AddCriteria(IEnumerable<Criteria> criteria)
        {
            var searchableProperties = GetSearchableProperties().ToArray();
            var validCriteria = criteria.Where(c => searchableProperties.Contains(c.ColumnName));
            _criteria.AddRange(validCriteria);
        }

        protected virtual void AddOrderBy(IEnumerable<OrderByColumn> columns)
        {
            var sortableProperties = GetSortableProperties().ToArray();
            var validOrderBy = columns.Where(c => sortableProperties.Contains(c.ColumnName));
            _orderBy.AddRange(validOrderBy);
        }

        protected virtual void AddPaging(int limit, int offset)
        {
            IsPagingEnabled = true;
            Limit = limit;
            Offset = offset;
        }

        private static IEnumerable<string> GetSearchableProperties() =>
            typeof(T)
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<Searchable>() != null)
                .Select(prop => prop.Name);

        private static IEnumerable<string> GetSortableProperties() =>
            typeof(T)
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<Sortable>() != null)
                .Select(prop => prop.Name);
    }
}
