using System;
using System.Collections.Generic;
using System.Text;
using Helpers.Auth.AuthHelper.Rules;

namespace Helpers.Auth.AuthHelper
{
    public class AuthHelperBuilder
    {
        private AuthHelper _authHelper = new AuthHelper();

        public AuthHelperBuilder AllowSystem()
        {
            _authHelper.AddRule(new AllowIdRule(AuthorizationConstants.System));
            return this;
        }

        public AuthHelperBuilder AllowId(string id)
        {
            _authHelper.AddRule(new AllowIdRule(id));
            return this;
        }

        public AuthHelperBuilder AllowPermissions(params string[] permissions)
        {
            _authHelper.AddRule(new AllowPermissionsRule(permissions));
            return this;
        }
        
        public AuthHelperBuilder RequireId(string id)
        {
            _authHelper.AddRule(new RequireIdRule(id));
            return this;
        }

        public AuthHelperBuilder RequirePermissions(params string[] permissions)
        {
            _authHelper.AddRule(new RequirePermissionsRule(permissions));
            return this;
        }

        public IAuthHelper Build()
            => _authHelper;
    }
}
