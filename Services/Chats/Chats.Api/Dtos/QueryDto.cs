using System.Collections.Generic;
using Chats.Api.Infrastructure.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Chats.Api.Dtos
{
    [ModelBinder(BinderType = typeof(QueryDtoBinder))]
    public class QueryDto
    {
        public IDictionary<string, string> Criteria { get; set; } = new Dictionary<string, string>();

        public PaginationDto Pagination { get; set; }
    }
}
