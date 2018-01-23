using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace SocialNetwork.OAuth.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> All()
        {
            return new[] {
                new Client
                {
                    ClientId = "socialnetwork",
                    ClientName = "SocialNetwork API",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "socialnetwork" }
                },
                new Client
                {
                    ClientId = "socialnetwork_implicit",                    
                    ClientName = "SocialNetwork Web (Access Token)",
                    ClientSecrets = new [] { new Secret("secret.web".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,    // flows = decide how ID token and Access token are returned to the client
                    AllowedScopes = new [] {
                        // these are the identity resources
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "office",
                        // these are the api resources
                        "socialnetwork",
                        "socialnetwork.api.read",
                        "socialnetwork.api.write",
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://localhost:1745/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://localhost:1745/signout-callback-oidc" },
                    // to support logging out all clients when there is a logout request from one of the clients (done using iFrame)
                    LogoutUri = "http://localhost:1745/signout-oidc"
                },
                new Client
                {
                    ClientId = "socialnetwork_code",
                    ClientName = "SocialNetwork Web (Code)",
                    ClientSecrets = new [] { new Secret("secret.code".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = new [] {
                        // these are the identity resources
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "office",
                        // these are the api resources
                        "socialnetwork",
                        "socialnetwork.api.read",
                        "socialnetwork.api.write",
                    },
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RedirectUris = new [] { "http://localhost:1745/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://localhost:1745/signout-callback-oidc" }
                }
            };
        }
    }
    public class Users
    {
        public static List<TestUser> All()
        {
            return new List<TestUser> {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "tandi",
                    Password = "password",
                    Claims = new List<Claim>
                    {
                        new Claim("email", "tandi.sunarto@hotmail.com")
                    }
                }
            };
        }
    }

    public class InMemoryConfiguration
    {
        public static IEnumerable<Client> Clients()
        {
            return new[] {
                new Client
                {
                    ClientId = "socialnetwork",
                    ClientName = "SocialNetwork API",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "socialnetwork" }
                },
                new Client
                {
                    ClientId = "socialnetwork_implicit",
                    ClientName = "SocialNetwork Web (Access Token)",
                    ClientSecrets = new [] { new Secret("secret.web".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,    // flows = decide how ID token and Access token are returned to the client
                    AllowedScopes = new [] {
                        // these are the identity resources
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "office",
                        // these are the api resources
                        "socialnetwork",
                        "socialnetwork.api.read",
                        "socialnetwork.api.write",
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://localhost:1745/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://localhost:1745/signout-callback-oidc" },
                    // to support logging out all clients when there is a logout request from one of the clients (done using iFrame)
                    LogoutUri = "http://localhost:1745/signout-oidc"
                },
                new Client
                {
                    ClientId = "socialnetwork_code",
                    ClientName = "SocialNetwork Web (Code)",
                    ClientSecrets = new [] { new Secret("secret.code".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = new [] {
                        // these are the identity resources
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "office",
                        // these are the api resources
                        "socialnetwork",
                        "socialnetwork.api.read",
                        "socialnetwork.api.write",
                    },
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RedirectUris = new [] { "http://localhost:1745/signin-oidc" },
                    PostLogoutRedirectUris = new [] { "http://localhost:1745/signout-callback-oidc" }
                }
            };
        }


        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[] {
                new ApiResource("socialnetwork", "Social Network"),
                new ApiResource("socialnetwork.api", "My SocialNetwork API")
                {
                    Scopes = new List<Scope>
                    {
                        new Scope
                        {
                            Name = "socialnetwork.api.read",
                            DisplayName = "SocialNetwork API to read employee and inventory records",
                        },
                        new Scope("socialnetwork.api.write")
                        {
                            DisplayName = "SocialNetwork API to update database records",
                            UserClaims = new List<string> {
                                "employee_update",
                                "inventory_update",
                                "email"
                            }
                        },
                        new Scope("socialnetwork.api.delete", "SocialNetwork API to delete database records")
                    }
                }
            };
        }

        // detail that identity server is protecting
        public static IEnumerable<IdentityResource> IdentityResources() 
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "office",
                    DisplayName = "Your Office Info",
                    UserClaims =
                    {
                        "office_number",
                        "office_location"
                    }
                }
            };
        }
    }
}
