using System;
using Helpers.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Chats.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationWithBypass(this IServiceCollection services,
            Action<JwtBearerOptions> configureJwtBearer, Action<AuthenticationWithBypassOptions> configureBypass)
        {
            var options = new AuthenticationWithBypassOptions();
            configureBypass(options);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configureJwtBearer)
                .AddScheme<MagicHeaderAuthenticationOptions, MagicHeaderAuthenticationHandler>(MagicHeaderDefaults.AuthenticationScheme, opt =>
                {
                    opt.Header = options.BypassAuthenticationHeader;
                    opt.Secret = options.BypassAuthenticationSecret;
                });

            // Configure the default authorization policy to use two schemes in an OR fashion
            services.AddAuthorization(opt =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme,
                    MagicHeaderDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                opt.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            return services;
        }
    }
}
