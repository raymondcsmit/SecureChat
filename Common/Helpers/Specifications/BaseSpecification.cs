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
        public string ColumnName { get; set; }
        public string TableName { get; }
        public string Value { get; }

        public Criteria(string propertyName, string tableName, string value)
        {
            ColumnName = propertyName.ToLowerInvariant();
            TableName = tableName;
            Value = value;
        }
    }

    public class OrderByColumn
    {
        public string ColumnName { get; set; }

        public string Mode { get; }

        public OrderByColumn(string columnName, string mode)
        {
            if (!new[] { OrderByMode.Ascending, OrderByMode.Descending }.Contains(mode))
            {
                throw new ArgumentException();
            }

            ColumnName = columnName.ToLowerInvariant();
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
            var searchableProperties = GetProperties<Searchable>()
                .ToDictionary(tup => tup.propName, tup => tup.colName);
            
            var validCriteria = criteria
                .Where(crit => searchableProperties.ContainsKey(crit.ColumnName))
                .Select(crit =>
                {
                    crit.ColumnName = searchableProperties[crit.ColumnName] ?? crit.ColumnName;
                    return crit;
                });

            _criteria.AddRange(validCriteria);
        }

        protected virtual void AddOrderBy(IEnumerable<OrderByColumn> columns)
        {
            var sortableProperties = GetProperties<Sortable>()
                .ToDictionary(tup => tup.propName, tup => tup.colName);
            
            var validOrderBy = columns
                .Where(oby => sortableProperties.ContainsKey(oby.ColumnName))
                .Select(oby =>
                {
                    oby.ColumnName = sortableProperties[oby.ColumnName] ?? oby.ColumnName;
                    return oby;
                });

            _orderBy.AddRange(validOrderBy);
        }

        protected virtual void AddPaging(int limit, int offset)
        {
            IsPagingEnabled = true;
            Limit = limit;
            Offset = offset;
        }

        private static IEnumerable<(string propName, string colName)> GetProperties<TAttribute>() where TAttribute: SpecificationAttribute =>
            typeof(T)
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<TAttribute>() != null)
                .Select(prop => (prop.Name.ToLowerInvariant(), prop.GetCustomAttribute<TAttribute>().ColumnName));
    }
}
