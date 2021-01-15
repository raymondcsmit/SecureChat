using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper.Rules
{
    public class AllowPermissionsRule : Rule<IIdentityService, bool>
    {
        private readonly IEnumerable<string> _requiredPermissions;

        public AllowPermissionsRule(IEnumerable<string> requiredPermissions)
        {
            _requiredPermissions = requiredPermissions;
        }

        public override async Task<bool> ApplyAsync(IIdentityService identityService, CancellationToken ct = default)
        {
            var permissions = identityService.GetPermissions();
            var hasPermissions = !_requiredPermissions.Except(permissions).Any();
            if (hasPermissions)
            {
                if (Next == null)
                {
                    return true;
                }
                return await Next.ApplyAsync(identityService);
            }
            return false;
        }
    }
}
