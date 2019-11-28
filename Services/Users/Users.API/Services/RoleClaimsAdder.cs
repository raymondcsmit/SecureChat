using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Users.API.Models;

namespace Users.API.Services
{
    public class RoleClaimsAdder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimsAdder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddRoleClaimsAsync(string userId, params string[] roleNames)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException();
            await AddRoleClaimsAsync(user, roleNames);
        }

        public async Task AddRoleClaimsAsync(User user, params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName) ?? throw new InvalidOperationException();
                await _userManager.AddClaimsAsync(user, await _roleManager.GetClaimsAsync(role));
            }
        }
    }
}
