using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Helpers.Auth
{
    public class MagicHeaderAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Header { get; set; }
        public string Secret { get; set; }
    }
}
