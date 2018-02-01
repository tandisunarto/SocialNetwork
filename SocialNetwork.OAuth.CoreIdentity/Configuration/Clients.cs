using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace SocialNetwork.OAuth.Configuration
{
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
                        new Claim(JwtClaimTypes.Name, "Tandi Sunarto"),
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
                    ClientId = "socialnetwork_coreidentity",
                    ClientName = "SocialNetwork Web CoreIdentity",
                    ClientSecrets = { new Secret("socialnetwork_coreidentity".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,

                    RedirectUris = { "http://localhost:1810/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:1810/signout-callback-oidc" },
                    //RequireConsent = false,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "socialnetwork.client_credentials",
                    ClientName = "Testing with Postman",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    },
                },
                new Client
                {
                    ClientId = "socialnetwork.password",
                    ClientName = "Testing with Postman",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    },
                },
                new Client
                {
                    ClientId = "socialnetwork.implicit",
                    ClientName = "Testing with Postman",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    },
                },
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
