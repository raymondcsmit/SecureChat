using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper
{
    public class AuthHelper : IAuthHelper
    {
        private Rule<IIdentityService, bool> _rules = new RuleProcessor<IIdentityService, bool>();

        public void AddRule(Rule<IIdentityService, bool> rule) 
            => _rules.AddRule(rule);

        public bool Authorize(IIdentityService identityService)
            => _rules.ApplyAsync(identityService).Result;
    }
}
