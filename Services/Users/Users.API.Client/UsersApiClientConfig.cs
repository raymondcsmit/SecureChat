using System;
using System.Collections.Generic;
using System.Text;

namespace Users.API.Client
{
    public class UsersApiClientConfig
    {
        public string UsersServiceUrl { get; set; }

        public string BypassAuthenticationHeader { get; set; }

        public string BypassAuthenticationSecret { get; set; }
    }
}
