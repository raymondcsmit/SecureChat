using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration.Infrastructure
{
    public static class API
    {
        public static class Users
        {
            public static string RegisterUser(string baseUri) => $"{baseUri}/users";

            public static string VerifyEmail(string baseUri) => $"{baseUri}/users/email-verification";

            public static string RequestPasswordReset(string baseUri) => $"{baseUri}/users/password-change-request";

            public static string ResetPassword(string baseUri) => $"{baseUri}/users/password-change";
        }
    }
}
