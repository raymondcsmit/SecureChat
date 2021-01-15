using System.Collections.Generic;
using Users.API.Application.Specifications;

namespace Users.UnitTests.Specifications
{
    public class TestSpecification : BaseSpecification<TestModel>
    {
        public void AddCriteria(IEnumerable<Criteria> criteria)
        {
            base.AddCriteria(criteria);
        }

        public void AddOrderBy(IEnumerable<OrderByColumn> columns)
        {
            base.AddOrderBy(columns);
        }

        public void AddPaging(int take, int skip)
        {
            base.AddPaging(take, skip);
        }
    }
}
