using System.Collections.Generic;
using System.Security.Claims;

namespace Account.API.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();

        IEnumerable<Claim> GetClaims();

        IEnumerable<string> GetPermissions();
    }
}
