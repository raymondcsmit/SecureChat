using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Helpers.Auth;
using Microsoft.AspNetCore.Http;

namespace Chats.Api.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetUserIdentity() =>
            _context.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        public IEnumerable<Claim> GetClaims() =>
            _context.HttpContext.User.Claims;

        public IEnumerable<string> GetPermissions() =>
            GetClaims()
                .Where(claim => claim.Type == "permission")
                .Select(claim => claim.Value);
    }
}
