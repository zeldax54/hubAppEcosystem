using DTO;
using DTOS;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Midgard
{
    public interface ITokenProviders
    {
        public Task<ResponseToken> GetAPIToken();
        public Task<ResponseToken> GetUserToken(string user,string password);

    }
    public class TokenProviders : ITokenProviders
    {
        private readonly IConfiguration _configuration;

        public TokenProviders(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<ResponseToken>  GetAPIToken()
        {
            var apiInfo = _configuration.GetSection("ApiInfo").Get<APiInfo>();
            var tokenUrl = _configuration.GetSection("TokenProviders").Get<string>();
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(tokenUrl);
            ClientCredentialsTokenRequest clientCredentialsRequest = new ClientCredentialsTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = apiInfo.client_id,
                ClientSecret = apiInfo.client_secret,
                GrantType = apiInfo.grant_type
            };
            var response = await client.RequestClientCredentialsTokenAsync(clientCredentialsRequest);
            return new ResponseToken() {             
                Token = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }

        public async Task<ResponseToken> GetUserToken(string user, string password)
        {
            var userInfo = _configuration.GetSection("UserInfo").Get<UserInfo>();
            var tokenUrl = _configuration.GetSection("TokenProviders").Get<string>();
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(tokenUrl);
            PasswordTokenRequest passwordTokenRequest = new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = userInfo.client_id,
                GrantType = userInfo.grant_type,
                UserName = user,
                Password = password
            };
            var response = await client.RequestPasswordTokenAsync(passwordTokenRequest);
            return new ResponseToken()
            {
                Token = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }
    }
}
