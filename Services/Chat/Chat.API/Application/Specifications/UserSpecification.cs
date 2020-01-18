using System.Linq;
using Chat.API.Dtos;
using Helpers.Specifications;

namespace Chat.API.Application.Specifications
{
    public class UserSpecification : BaseSpecification<UserDto>
    {
        public UserSpecification(QueryDto query)
        {
            var criteria = query.Criteria.Select(kvp => new Criteria(kvp.Key, "Users", kvp.Value));
            base.AddCriteria(criteria);
            if (query.Pagination != null)
            {
                base.AddPaging(query.Pagination.Limit, query.Pagination.Offset);
            }
        }
    }
}
