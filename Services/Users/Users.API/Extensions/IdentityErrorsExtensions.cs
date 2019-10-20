using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Users.API.Extensions
{
    public static class IdentityErrorsExtensions
    {
        public static string ToErrorString(this IEnumerable<IdentityError> errors) 
            => string.Join("\n", errors);
    }
}
