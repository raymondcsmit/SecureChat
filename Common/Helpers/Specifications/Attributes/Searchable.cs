using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications.Attributes
{
    public class Searchable : SpecificationAttribute
    {
        public Searchable(string columnName = null) : base(columnName)
        {
        }
    }
}
