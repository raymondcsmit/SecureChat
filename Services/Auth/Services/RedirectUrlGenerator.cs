using System.Collections.Specialized;
using Auth.Controllers;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Auth.Services
{
    public class RedirectUrlGenerator: IRedirectUrlGenerator
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public RedirectUrlGenerator(
            IUrlHelperFactory urlHelperFactory, 
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        public string GenerateUrl(string baseUrl)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var httpContext = _httpContextAccessor.HttpContext;

            var returnUrl = httpContext.Request.Query["returnUrl"];
            var loginUrl = urlHelper.Link(nameof(AccountController.Login), new {returnUrl = returnUrl});

            return UriHelpers.BuildUri(baseUrl, new NameValueCollection()
            {
                { "loginUrl", loginUrl }
            })
            .ToString();
        }
    }
}
