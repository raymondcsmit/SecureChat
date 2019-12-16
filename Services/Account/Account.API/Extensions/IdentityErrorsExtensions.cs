using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Account.API.Extensions
{
    public static class IdentityErrorsExtensions
    {
        public static string ToErrorString(this IEnumerable<IdentityError> errors) 
            => string.Join("\n", errors);
    }
}
