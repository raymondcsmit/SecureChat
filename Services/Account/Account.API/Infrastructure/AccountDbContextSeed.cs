using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Account.API.Application.IntegrationEvents.Events;
using Account.API.Models;
using Account.API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SecureChat.Common.Events.EventBus.Abstractions;

namespace Account.API.Infrastructure
{
    public class AccountDbContextSeed
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

        private readonly AccountDbContext _accountDbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountDbContextSeed> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly RolePermissionsService _rolePermissionsService;
        private readonly IEventBus _eventBus;

        public AccountDbContextSeed(
            AccountDbContext accountDbContext,
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountDbContextSeed> logger,
            IHostingEnvironment environment,
            RolePermissionsService rolePermissionsService,
            IEventBus eventBus)
        {
            _accountDbContext = accountDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _environment = environment;
            _rolePermissionsService = rolePermissionsService;
            _eventBus = eventBus;
        }

        public async Task SeedAsync()
        {
            if (_roleManager.Roles.Any())
            {
                _logger.LogInformation("Account DB roles already seeded. Aborting.");
                return;
            }
            else
            {
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
            }

            if ( _userManager.Users.Any())
            {
                _logger.LogInformation("Account DB users already seeded. Aborting.");
                return;
            }
            else
            {
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
                        await _rolePermissionsService.AddRolePermissionsAsync(user, userInfo.Roles.ToArray());
                        _eventBus.Publish(new UserAccountCreatedIntegrationEvent(user.Id, user.UserName, user.Email));
                    }
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
