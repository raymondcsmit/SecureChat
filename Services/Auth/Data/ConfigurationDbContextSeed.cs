using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Auth.Data
{
    public class ConfigurationDbContextSeed
    {
        private readonly ConfigurationDbContext _context;
        private readonly AuthSettings _authSettings;

        public ConfigurationDbContextSeed(ConfigurationDbContext context, IOptions<AuthSettings> oidcSettings)
        {
            _context = context;
            _authSettings = oidcSettings.Value;
        }

        public async Task SeedAsync()
        {
            if (!_context.Clients.Any())
            {
                await _context.Clients.AddRangeAsync(GetClients());
            }
            
            if (!_context.IdentityResources.Any())
            {
                await _context.IdentityResources.AddRangeAsync(GetIdentityResources());
            }

            if (!_context.ApiResources.Any())
            {
                await _context.ApiResources.AddRangeAsync(GetApiResources());
            }

            await _context.SaveChangesAsync();
        }

        private IEnumerable<Client> GetClients()
        {
            var props = _authSettings.GetType()
                .GetProperties()
                .Where(prop => prop.Name.Contains("Client"));
            var clientUrls = props.ToDictionary(prop => prop.Name, prop => prop.GetValue(_authSettings) as string);

            return Config.GetClients(clientUrls)
                .Select(model => model.ToEntity());
        }

        private IEnumerable<IdentityResource> GetIdentityResources() =>
            Config.GetIdentityResources()
                .Select(model => model.ToEntity());

        private IEnumerable<ApiResource> GetApiResources() =>
            Config.GetApiResources()
                .Select(model => model.ToEntity());
    }
}
