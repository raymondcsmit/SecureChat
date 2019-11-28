using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Users.API.Models;
using Users.API.Services;

namespace Users.API.Infrastructure
{
    public class UsersDbContextSeed
    {
        private class RoleInfo
        {
            public string Name { get; set; }
            public IEnumerable<string> Permissions { get; set; }
        }

        private class UserInfo
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public IEnumerable<string> Roles { get; set; }
        }

        private readonly UsersDbContext _usersDbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsersDbContextSeed> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly RoleClaimsAdder _roleClaimsAdder;

        public UsersDbContextSeed(
            UsersDbContext usersDbContext,
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersDbContextSeed> logger,
            IHostingEnvironment environment,
            RoleClaimsAdder roleClaimsAdder)
        {
            _usersDbContext = usersDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _environment = environment;
            _roleClaimsAdder = roleClaimsAdder;
        }

        public async Task SeedAsync()
        {
            if (_roleManager.Roles.Any() || _userManager.Users.Any())
            {
                _logger.LogInformation("Identity DB already seeded. Aborting.");
                return;
            }

            var roles = await LoadRoles();
            foreach (var roleInfo in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleInfo.Name);
                if (role == null)
                {
                    role = new IdentityRole(roleInfo.Name);
                    await _roleManager.CreateAsync(role);
                    var permissionClaims = roleInfo.Permissions
                        .Select(perm => new Claim("permission", perm))
                        .ToList();
                    foreach (var claim in permissionClaims)
                    {
                        await _roleManager.AddClaimAsync(role, claim);
                    }
                }
            }

            foreach (var userInfo in await LoadUsers())
            {
                var user = await _userManager.FindByNameAsync(userInfo.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = userInfo.UserName,
                        Email = userInfo.Email
                    };
                    await _userManager.CreateAsync(user, userInfo.Password);
                    user = await _userManager.FindByNameAsync(userInfo.UserName);
                    await _roleClaimsAdder.AddRoleClaimsAsync(user, userInfo.Roles.ToArray());
                }
            }
        }

        private async Task<IEnumerable<RoleInfo>> LoadRoles()
        {
            using (var file = File.OpenText($@"{_environment.ContentRootPath}/Setup/Roles.json"))
            {
                var content = await file.ReadToEndAsync();
                var roles = JsonConvert.DeserializeObject<List<RoleInfo>>(content);
                return roles;
            }
        }

        private async Task<IEnumerable<UserInfo>> LoadUsers()
        {
            using (var file = File.OpenText($@"{_environment.ContentRootPath}/Setup/Users.json"))
            {
                var content = await file.ReadToEndAsync();
                var users = JsonConvert.DeserializeObject<List<UserInfo>>(content);
                return users;
            }
        }
    }
}
