using System;
using System.Collections.Generic;
using System.Text;
using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper
{
    public interface IAuthHelper
    {
        void AddRule(Rule<IIdentityService, bool> rule);

        bool Authorize(IIdentityService identityService);
    }
}
