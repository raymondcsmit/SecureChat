using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Registration.Services
{
    public class LoginUrlService : ILoginUrlService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginUrlService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetLoginUrlCookie(string loginUrl)
        {
            _httpContextAccessor.HttpContext.Session.SetString("loginUrl", loginUrl);
        }

        public string GetLoginUrl()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("loginUrl");
        }

        public void ClearLoginUrlCookie()
        {
            _httpContextAccessor.HttpContext.Session.Remove("loginUrl");
        }
    }
}
