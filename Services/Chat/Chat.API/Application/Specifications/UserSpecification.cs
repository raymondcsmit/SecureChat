using System.Collections.Generic;
using System.Linq;
using Chat.API.Dtos;
using Helpers.Specifications;

namespace Chat.API.Application.Specifications
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
                base.AddCriteriaNot(criteriaNot);
            }

            if (query.Pagination != null)
            {
                base.AddPaging(query.Pagination.Limit, query.Pagination.Offset);
            }
        }
    }
}
