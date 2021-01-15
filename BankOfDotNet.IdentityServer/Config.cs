using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace BankOfDotNet.IdentityServer
{
    public class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "Babak",
                    Password = "babak@",
                },
                new TestUser
                {
                    Username = "Bob",
                    Password = "bob@",
                    SubjectId = "2"
                }
            };
        }

        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile() // <-- useful
            };
        }

        public static IEnumerable<ApiScope> Scopes
        {
            get
            {
                return new List<ApiScope> { new ApiScope("BankOfDotNetApi", "Bank Api Scope") };
            }
        }

        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("BankApi","Customer Api For Bank of Dotnet"){Scopes = {"BankOfDotNetApi"}}
            };
        }

        public static IEnumerable<Client> GetAllClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("Secret".Sha256())},
                    AllowedScopes = { "BankOfDotNetApi" }
                },

                new Client
                {
                    ClientId = "T.Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "BankOfDotNetApi" },
                    ClientSecrets = new List<Secret>{new Secret("secret".Sha256())}
                },

                new Client
                {
                    ClientId = "mvc",
                    ClientName = "Mvc Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "https://localhost:5501/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5501/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile
                    }
                },

                new Client
                {
                    ClientId = "swaggerapiui",
                    ClientName = "Swagger Api UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = {"https://localhost:5001/swagger/oauth2-redirect.html"},
                    PostLogoutRedirectUris = { "https://localhost:5001/swagger" },
                    AllowedScopes = { "BankOfDotNetApi" },
                    AllowAccessTokensViaBrowser = true
                }
            };
        }
    }
}
