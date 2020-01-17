using Helpers.Specifications.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.UnitTests.Specifications
{
    public class TestModel
    {
        [Searchable]
        public string Foo { get; }

        [Sortable]
        public string Bar { get; }

        [Searchable, Sortable]
        public string Baz { get; }
    }
}
