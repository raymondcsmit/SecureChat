using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Auth
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("account", "Account Service")
                {
                    UserClaims = new[] { "permission", "sub" },

                },
                new ApiResource("chat", "Chat Service")
                {
                    UserClaims = new[] { "permission", "sub" }
                }
            };
        }

        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientUrls)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "AngularSpaClient",
                    ClientName = "SecureChat Angular App",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowRememberConsent = false,

                    RedirectUris =           { $"{clientUrls["AngularSpaClientUrl"]}/auth/sign-in-callback" },
                    PostLogoutRedirectUris = { $"{clientUrls["AngularSpaClientUrl"]}/auth/sign-out-callback" },
                    AllowedCorsOrigins =     { clientUrls["AngularSpaClientUrl"] },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        "permissions",
                        "account",
                        "chat",
                        "email_confirmed"
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                // Standard scopes
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource("permissions", "Permissions", new[] {"permission"}),
                new IdentityResource("email_confirmed", "Email Confirmation Status", new[] {"email_confirmed"})
            };
        }
    }
}
