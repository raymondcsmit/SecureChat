using Users.API.Application.Specifications.Attributes;

namespace Users.UnitTests.Specifications
{
    public class TestModel
    {
        [Searchable(ColumnName = "foo")]
        public string Foo { get; }

        [Sortable]
        public string Bar { get; }

        [Searchable, Sortable]
        public string Baz { get; }
    }
}
