using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Authorization.Web.Configs
{
    public class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResourceResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), //必须要添加，否则报无效的scope错误
                new IdentityResources.Profile()
            };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("netcore.api", "NetCore.API")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "netcore.api" }
                },
                new Client
                {
                    ClientId = "pwd.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "netcore.api",IdentityServerConstants.StandardScopes.OpenId, //必须要添加，否则报forbidden错误
                  IdentityServerConstants.StandardScopes.Profile }
                }

                //new Client
                //{
                //    ClientId = "ro.Client",
                //    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },
                //    AllowedScopes = {"UserApi"}
                //},
                //   // OpenID Connect implicit flow client (MVC)
                //new Client
                //{
                //    ClientId = "MVC",
                //    ClientName = "MVC Client",
                //    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                //     ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },
                //    RedirectUris = { "http://localhost:5002/signin-oidc" },
                //    PostLogoutRedirectUris = { "http://localhost:5002" },

                //    AllowedScopes =
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "UserApi"
                //    },
                //    AllowOfflineAccess = true
                //},
                //   // JavaScript Client
                //new Client
                //{
                //    ClientId = "js",
                //    ClientName = "JavaScript Client",
                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    AllowAccessTokensViaBrowser = true,

                //    RedirectUris = { "http://localhost:5003/callback.html" },
                //    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                //    AllowedCorsOrigins = { "http://localhost:5003" },

                //    AllowedScopes =
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "UserApi"
                //    },
                //}
            };
        }

        public static List<TestUser> GeTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "qwerty",
                    Password = "a123"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "aspros",
                    Password = "b123"
                }
            };
        }
    }
}
