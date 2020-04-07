using Users.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Specifications;

namespace Users.API.Application.Specifications
{
    public class FriendshipRequestSpecification : BaseSpecification<FriendshipRequestDto>
    {
        public FriendshipRequestSpecification(string id)
        {
            var criteria = new[] { new Criteria("id", "FriendshipRequests", id) };
            base.AddCriteria(criteria);
        }

        public FriendshipRequestSpecification(QueryDto query, string requesteeId = null)
        {
            var criteria = query.Criteria?.Select(kvp => new Criteria(kvp.Key, "FriendshipRequests", kvp.Value));
            base.AddCriteria(criteria);

            if (requesteeId != null)
            {
                var requesterCriteria = new Criteria("Requestee", "FriendshipRequests", requesteeId);
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

            var requesterCriteria = new Criteria("Requestee", "FriendshipRequests", requesteeId);
            var requesteeCriteria = new Criteria("Requester", "FriendshipRequests", requesterId);
            base.AddCriteria(new[] { requesterCriteria, requesteeCriteria });
        }
    }
}
