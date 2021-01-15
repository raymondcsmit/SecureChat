using System.Collections.Generic;
using System.Linq;
using Users.API.Dtos;

namespace Users.API.Application.Specifications
{
    public class UserSpecification : BaseSpecification<UserDto>
    {
        public UserSpecification(QueryDto query, IEnumerable<string> exclude = null)
        {
            var criteria = query.Criteria.Select(kvp => new Criteria(kvp.Key, "Users", kvp.Value));
            base.AddCriteria(criteria);

            var criteriaNot = exclude?.Select(id => new Criteria("id", "Users", id));
            if (exclude != null)
            {
                base.AddExcludeCriteria(criteriaNot);
            }

            if (query.Pagination != null)
            {
                base.AddPaging(query.Pagination.Limit, query.Pagination.Offset);
            }
        }
    }
}
