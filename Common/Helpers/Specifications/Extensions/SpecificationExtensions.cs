using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Specifications.Extensions
{
    public static class SpecificationExtensions
    {
        public static string Apply<T>(this ISpecification<T> spec, string baseQuery) 
            => $@"{baseQuery.Trim(';')}
                      {spec.Criteria}
                      {spec.OrderBy}
                      {spec.Pagination};";
    }
}
