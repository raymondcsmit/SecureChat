using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Registration.Services
{
    public class UrlGeneratorService : IUrlGeneratorService
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public UrlGeneratorService(
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        public string GetUrl(string actionName)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.Link(actionName, new { });
        }

        public string GetUrl(string actionName, object queryParams)
        {
            var baseUrl = GetUrl(actionName);
            return UriHelpers.BuildUri(baseUrl, queryParams).ToString();
        }
    }
}
