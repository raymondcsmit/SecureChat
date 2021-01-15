using Users.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Users.API.Application.Specifications
{
    public class FriendshipSpecification : BaseSpecification<FriendshipDto>
    {
        public FriendshipSpecification(string id)
        {
            var criteria = new[] { new Criteria("Id", "Friendships", id) };
            base.AddCriteria(criteria);
        }

        public FriendshipSpecification(QueryDto query)
        {
            var criteria = query.Criteria?.Select(kvp => new Criteria(kvp.Key, "Friendships", kvp.Value));
            base.AddCriteria(criteria);

            if (query.Pagination != null)
            {
                base.AddPaging(query.Pagination.Limit, query.Pagination.Offset);
            }
        }

        public FriendshipSpecification(string user1, string user2)
        {
            if (user1 == null && user2 == null)
            {
                throw new InvalidOperationException("Both users cannot be null");
            }

            var criteria = new List<Criteria>();
            if (user1 != null)
            {
                criteria.Add(new Criteria("User1", "Friendships", user1));

            }
            if (user2 != null)
            {
                criteria.Add(new Criteria("User2", "Friendships", user2));

            }
            base.AddCriteria(criteria);
        }
    }
}
