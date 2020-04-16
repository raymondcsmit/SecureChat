using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Users.API.Infrastructure.Binders;
using Helpers.Mapping;
using Helpers.Specifications;
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
