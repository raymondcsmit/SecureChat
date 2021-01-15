using System.Collections.Generic;
using Users.API.Infrastructure.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Users.API.Dtos
{
    [ModelBinder(BinderType = typeof(QueryDtoBinder))]
    public class QueryDto
    {
        public IDictionary<string, string> Criteria { get; set; } = new Dictionary<string, string>();

        public PaginationDto Pagination { get; set; }
    }
}
