using System;

namespace Users.API.Application.Specifications.Attributes
{
    public abstract class SpecificationAttribute : Attribute
    {
        public SpecificationAttribute(string columnName = null)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; set; }
    }
}
