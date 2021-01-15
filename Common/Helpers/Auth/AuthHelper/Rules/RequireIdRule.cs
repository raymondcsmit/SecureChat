using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper.Rules
{
    public class RequireIdRule : Rule<IIdentityService, bool>
    {
        private readonly string _id;

        public RequireIdRule(string id)
        {
            _id = id;
        }

        public override async Task<bool> ApplyAsync(IIdentityService identityService, CancellationToken ct = default)
        {
            return identityService.GetUserIdentity() == _id;
        }
    }
}
