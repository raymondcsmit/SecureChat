using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications.Attributes
{
    public abstract class SpecificationAttribute : Attribute
    {
        public SpecificationAttribute(string columnName = null)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; }
    }
}
