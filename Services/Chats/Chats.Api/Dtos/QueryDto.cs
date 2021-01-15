using System.Collections.Generic;
using Chats.Api.Infrastructure.Binders;
using Chats.Domain.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Chats.Api.Dtos
{
    [ModelBinder(BinderType = typeof(QueryDtoBinder))]
    public class QueryDto
    {
        public IEnumerable<StringCriterion> Criteria { get; set; } = new List<StringCriterion>();

        public IEnumerable<StringOrderBy> OrderBy { get; set; } = new List<StringOrderBy>();

        public PaginationDto Pagination { get; set; }
    }
}
