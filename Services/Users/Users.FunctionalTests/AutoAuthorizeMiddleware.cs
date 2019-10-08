using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Users.FunctionalTests
{
    public class AutoAuthorizeMiddleware
    {
        private const string IdentityId = "9e3163b9-1ae6-4652-9dc6-7898ab7b7a00";

        private readonly RequestDelegate _next;

        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("cookies");

            identity.AddClaim(new Claim("sub", IdentityId));
            identity.AddClaim(new Claim("unique_name", IdentityId));

            httpContext.User.AddIdentity(identity);

            await _next.Invoke(httpContext);
        }
    }

}
