using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

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
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "socialnetwork" }
                },
                new Client
                {
                    ClientId = "socialnetwork_implicit",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,    // flows = decide how ID token and Access token are returned to the client
                    AllowedScopes = new [] {
                        // identity resources
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "office_number",
                        // api resources
                        "socialnetwork",
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://localhost:1745/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:1745/signout-callback-oidc" },
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
                    Password = "password"
                }
            };
        }
    }

    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[] {
                new ApiResource("socialnetwork", "Social Network")
            };
        }

        // detail that identity server is protecting
        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "office",
                    UserClaims =
                    {
                        "office_number"
                    }
                }
            };
        }
    }
}
