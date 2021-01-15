using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper.Rules
{
    public class RequirePermissionsRule : Rule<IIdentityService, bool>
    {
        private readonly IEnumerable<string> _requiredPermissions;

        public RequirePermissionsRule(IEnumerable<string> requiredPermissions)
        {
            _requiredPermissions = requiredPermissions;
        }

        public override async Task<bool> ApplyAsync(IIdentityService identityService, CancellationToken ct = default)
        {
            var permissions = identityService.GetPermissions();
            var hasPermissions = !_requiredPermissions.Except(permissions).Any();
            return hasPermissions;
        }
    }
}
