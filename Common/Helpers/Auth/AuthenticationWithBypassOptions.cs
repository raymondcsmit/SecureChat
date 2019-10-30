using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Helpers.Auth
{
    public class AuthenticationWithBypassOptions
    {
        public string BypassAuthenticationHeader { get; set; }
        public string BypassAuthenticationSecret { get; set; }
    }
}
