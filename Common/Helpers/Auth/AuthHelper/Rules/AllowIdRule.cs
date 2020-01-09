using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper.Rules
{
    public class AllowIdRule: Rule<IIdentityService, bool>
    {
        private readonly string _id;

        public AllowIdRule(string id)
        {
            _id = id;
        }

        public override async Task<bool> ApplyAsync(IIdentityService identityService, CancellationToken ct = default)
        {
            if (identityService.GetUserIdentity() == _id)
            {
                return true;
            }
            return await Next.ApplyAsync(identityService);
        }
    }
}
