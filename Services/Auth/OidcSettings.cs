using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth
{
    public class OidcSettings
    {
        public string AngularSpaClientUrl { get; set; }

        public string RegistrationUrl { get; set; }

        public string PasswordResetUrl { get; set; }
    }
}
