using Chat.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Specifications;

namespace Chat.API.Application.Specifications
{
    public class FriendshipRequestSpecification : BaseSpecification<FriendshipRequestDto>
    {
        public FriendshipRequestSpecification(QueryDto query, string requesterId = null)
        {
            var criteria = query.Criteria.Select(kvp => new Criteria(kvp.Key, "FriendshipRequests", kvp.Value));
            base.AddCriteria(criteria);

            if (requesterId != null)
            {
                var requesterCriteria = new Criteria(nameof(UserDto.Id), "FriendshipRequests", requesterId);
                base.AddCriteria(new[] {requesterCriteria});
            }

            if (query.Pagination != null)
            {
                base.AddPaging(query.Pagination.Limit, query.Pagination.Offset);
            }
        }

        public FriendshipRequestSpecification(string requesterId, string requesteeId)
        {
            if (requesterId == null || requesteeId == null)
            {
                throw new InvalidOperationException();
            }

            var requesterCriteria = new Criteria(nameof(UserDto.Id), "FriendshipRequests", requesterId);
            var requesteeCriteria = new Criteria(nameof(UserDto.Id), "FriendshipRequests", requesteeId);
            base.AddCriteria(new[] { requesterCriteria, requesteeCriteria });
        }
    }
}
