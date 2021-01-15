using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Chats.Api.Dtos;
using Chats.Domain.AggregateModel;

namespace Chats.Api.Application.Specifications
{
    public class ChatSpecification : BaseSpecification<Chat>
    {
        public ChatSpecification(QueryDto query)
        {
            base.AddCriteria(query.Criteria);
            base.AddOrderBy(query.OrderBy);

            if (query.Pagination != null)
            {
                base.AddPagination(query.Pagination.Limit, query.Pagination.Offset);
            }

            base.AddInclude(chat => chat.Owner);
        }

        public new void AddCriteria(Expression<Func<Chat, bool>> criteriaExpression)
        {
            base.AddCriteria(criteriaExpression);
        }
    }
}
