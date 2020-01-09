using Helpers.RulesPattern;

namespace Helpers.Auth.AuthHelper
{
    public class AuthHelper : IAuthHelper
    {
        private RuleProcessor<IIdentityService, bool> _ruleProcessor = new RuleProcessor<IIdentityService, bool>();

        public void AddRule(Rule<IIdentityService, bool> rule) 
            => _ruleProcessor.AddRule(rule);

        public bool Authorize(IIdentityService identityService)
            => _ruleProcessor.ApplyAsync(identityService).Result;
    }
}
