using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity
{
    public static class Config
    {
        private static IEnumerable<ConfigClient> clients = Startup.StaticConfig.GetSection("Clients").Get<IEnumerable<ConfigClient>>();
        private static IEnumerable<string> _scopes = Startup.StaticConfig.GetSection("Scopes").Get<IEnumerable<string>>();

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", new[] { "role" })

            };
        }
        public static IEnumerable<ApiScope> Scopes()
        {
            return _scopes.Select(c => new ApiScope()
            {
                Name = c,           
            });
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            var resources = _scopes.Select(c => new ApiResource()
            {
                Name = c,
              
                Scopes = new List<string>
                {
                  c
                },
                UserClaims =
                {
                   JwtClaimTypes.Audience
                },
            }).ToList();

            return resources;

        }

        public static IEnumerable<Client> GetClients()
        {
            return clients.Select(a => new Client()
            {
                ClientId = a.ClientId,
                AllowedGrantTypes = a.AllowedGrantTypes,
                ClientSecrets = a.Secrets.Select(b => new Secret(b.ToString().Sha256())).ToList(),
                AllowedScopes = a.AllowedScopes,
                AllowAccessTokensViaBrowser = a.AllowAccessTokensViaBrowser,
                AccessTokenLifetime = a.AccessTokenLifetime,
                RequireClientSecret = a.RquireSecret,

                AllowOfflineAccess = a.AllowOfflineAccess,
                RefreshTokenUsage =  a.RefreshTokenUsage,
                RefreshTokenExpiration =  a.RefreshTokenExpiration

            }).ToList();
        }

       


        public class ConfigClient
        {
            public string ClientId { get; set; }
            public List<string> Secrets { get; set; }
            public List<string> AllowedScopes { get; set; }
            public List<string> AllowedGrantTypes { get; set; }
            public bool RquireSecret { get; set; }
            public bool AllowOfflineAccess { get; set; }
            public TokenUsage RefreshTokenUsage { get; set; }
            public TokenExpiration RefreshTokenExpiration { get; set; }
            public int AccessTokenLifetime { get; set; }
            public bool AllowAccessTokensViaBrowser { get; set; }


        }
    }
   
}
