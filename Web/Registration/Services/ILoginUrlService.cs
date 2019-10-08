using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration.Services
{
    public interface ILoginUrlService
    {
        void SetLoginUrlCookie(string loginUrl);
        string GetLoginUrl();
        void ClearLoginUrlCookie();
    }
}
