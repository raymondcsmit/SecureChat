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
                    UserClaims = { "permission", "sub" }
                },
                new ApiResource("users", "Users Service")
                {
                    UserClaims = { "permission", "sub" }
                },
                new ApiResource("messaging", "Messaging Service")
                {
                    UserClaims = { "permission", "sub" }
                },
                new ApiResource("session", "Session Service")
                {
                    UserClaims = { "permission", "sub" }
                },
                new ApiResource("chats", "Chats Service")
                {
                    UserClaims = { "permission", "sub" }
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

                    RedirectUris =           { $"{clientUrls["AngularSpaClientUrl"]}/auth/sign-in-callback", $"{clientUrls["AngularSpaClientUrl"]}/auth/sign-in-silent-callback" },
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
                        "users",
                        "messaging",
                        "session",
                        "chats"
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
            };
        }
    }
}
