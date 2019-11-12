using System.Collections.Generic;
using System.Security.Claims;

namespace Users.API.Infrastructure.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();

        IEnumerable<Claim> GetClaims();

        IEnumerable<string> GetPermissions();
    }
}
