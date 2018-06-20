using IdentityModel.Client;
using NetCore.Web.Commons.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Web.Commons
{
    public static class HttpRequestProvider
    {
        public static ApiSettings ApiSetting = new ApiSettings()
        {
            AuthorizeUrl = "http://localhost:6000",
            ClientId = "pwd.client",
            ClientSecret = "secret"

        };

        public static async Task<TokenInfo> GetTokenClaims(string userName,string password)
        {
            var token = new TokenInfo();
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync(ApiSetting.AuthorizeUrl);

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, ApiSetting.ClientId, ApiSetting.ClientSecret);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(userName, password);
            if (tokenResponse.IsError)
            {
                return token;
            }
            var userInfoClient = new UserInfoClient(disco.UserInfoEndpoint);
            var userResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);
            if (userResponse.IsError)
            {
                return token;
            }
            token.Token = tokenResponse.AccessToken;
            token.Claims = userResponse.Claims;
            return token;
        }
    }
}
