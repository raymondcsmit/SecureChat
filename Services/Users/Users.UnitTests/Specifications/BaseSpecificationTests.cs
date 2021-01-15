using System;
using System.Text.RegularExpressions;
using Users.API.Application.Specifications;
using Users.API.Application.Specifications.Extensions;
using Xunit;

namespace Users.UnitTests.Specifications
{
    public class BaseSpecificationTests
    {
        [Fact]
        public void SpecificationWithCriteria_IncludesSearchableColumns()
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddCriteria(new[] { new Criteria("Foo", "Table", "a"), new Criteria("Baz", "Table", "b") });
            (var query, dynamic preparedStatementObject) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize("SELECT * FROM Table WHERE Table.foo = @val0 AND Table.Baz = @val1;"));
            Assert.True(preparedStatementObject.val0 == "a");
            Assert.True(preparedStatementObject.val1 == "b");
        }

        [Fact]
        public void SpecificationWithCriteria_RejectsNotSearchableColumns()
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddCriteria(new[] { new Criteria("Foo", "Table", "a"), new Criteria("Bar", "Table", "b") });
            (var query, dynamic preparedStatementObject) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize("SELECT * FROM Table WHERE Table.foo = @val0;"));
            Assert.True(preparedStatementObject.val0 == "a");
            // Bar property should not exist
            Assert.ThrowsAny<Exception>(() => preparedStatementObject.val1);
        }

        [Theory]
        [InlineData(OrderByMode.Ascending)]
        [InlineData(OrderByMode.Descending)]
        public void SpecificationWithOrderBy_IncludesSortableColumns(string mode)
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddOrderBy(new[] { new OrderByColumn("Bar", mode), new OrderByColumn("Baz", mode)});
            var (query, _) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize($"SELECT * FROM Table ORDER BY Bar {mode}, Baz {mode};"));
        }

        [Theory]
        [InlineData(OrderByMode.Ascending)]
        [InlineData(OrderByMode.Descending)]
        public void SpecificationWithOrderBy_RejectsNotSortableColumns(string mode)
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddOrderBy(new[] { new OrderByColumn("Foo", mode), new OrderByColumn("Baz", mode) });
            var (query, _) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize($"SELECT * FROM Table ORDER BY Baz {mode};"));
        }

        [Theory]
        [InlineData(OrderByMode.Ascending)]
        [InlineData(OrderByMode.Descending)]
        public void SpecificationWithOrderBy_DoesNothingOnEmptyColumnList(string mode)
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddOrderBy(new[] { new OrderByColumn("gibberish1", mode), new OrderByColumn("gibberish2", mode) });
            var (query, _) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize($"SELECT * FROM Table;"));
        }

        [Fact]
        public void SpecificationWithPaging_AddsPaging()
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddPaging(10, 3);
            var (query, _) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize($"SELECT * FROM Table LIMIT 10 OFFSET 3;"));
        }

        [Theory]
        [InlineData(OrderByMode.Ascending)]
        [InlineData(OrderByMode.Descending)]
        public void SpecificationWithAllFeatures_WorksCorrectly(string mode)
        {
            var spec = new TestSpecification();
            var baseQuery = $@"SELECT * FROM Table;";
            spec.AddCriteria(new[] { new Criteria("Foo", "Table", "a"), new Criteria("Baz", "Table", "b") });
            spec.AddOrderBy(new[] { new OrderByColumn("Bar", mode), new OrderByColumn("Baz", mode) });
            spec.AddPaging(10, 3);
            (var query, dynamic preparedStatementObject) = spec.Apply(baseQuery);
            Assert.Equal(
                Normalize(query),
                Normalize($@"SELECT * FROM Table 
                            WHERE Table.foo = @val0 AND Table.Baz = @val1
                            ORDER BY Bar {mode}, Baz {mode}
                            LIMIT 10 OFFSET 3;"));
            Assert.True(preparedStatementObject.val0 == "a");
            Assert.True(preparedStatementObject.val1 == "b");
        }

        private string Normalize(string query)
            => Regex.Replace(query, @"\s+", "").ToLowerInvariant();

    }
}
