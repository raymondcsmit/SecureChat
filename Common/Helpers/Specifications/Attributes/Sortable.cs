using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications.Attributes
{
    public class Sortable : SpecificationAttribute
    {
        public Sortable(string columnName = null) : base(columnName)
        {
        }
    }
}
