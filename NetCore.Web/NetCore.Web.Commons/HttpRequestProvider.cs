using IdentityModel.Client;
using NetCore.Web.Commons.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Web.Commons
{
    public static class HttpRequestProvider
    {
        public static ApiSettings ApiSetting { get; set; }

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

        #region post异步请求
        /// <summary>
        /// post异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="paramList">List<KeyValuePair<String, String>> paramList</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<string> HttpRequestPostAsync(string url, string token,List<KeyValuePair<String, String>> paramList, int timeOut = 100000)
        {
            var result = string.Empty;
            //设置HttpClientHandler的AutomaticDecompression
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (HttpClient httpClient = new HttpClient(handler))
            {
                httpClient.SetBearerToken(token);
                var canTokens = new CancellationTokenSource();
                canTokens.CancelAfter(timeOut);
                
                //设置头
                //httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
                //httpClient.Timeout= TimeSpan.FromMilliseconds(timeOut);
                try
                {
                    //异步请求
                    var response = await httpClient.PostAsync(new Uri(url, UriKind.Absolute), new FormUrlEncodedContent(paramList), canTokens.Token);

                    //返回编码
                    var contentType = response.Content?.Headers.ContentType;
                    if (contentType !=null && string.IsNullOrEmpty(contentType?.CharSet))
                    {
                        contentType.CharSet = await GetCharSetAsync(response.Content) ?? "UTF8";
                    }

                    //获取返回结果
                    var readContent = await response.Content.ReadAsStringAsync();
                    result = readContent;
                }
                catch (WebException ex)
                {
                    result = ex.ToString();
                }
                catch (TaskCanceledException ex)
                {
                    if (!canTokens.Token.IsCancellationRequested)
                    {
                        result = "time out";
                    }
                    else
                    {
                        result = ex.ToString();
                    }
                }
                //用完要记得释放
                httpClient.Dispose();
                return result;
            }
        }
        #endregion

        #region 获取目标网页编码
        /// <summary>
        /// 获取目标网页编码
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static async Task<string> GetCharSetAsync(HttpContent httpContent)
        {
            var charset = httpContent.Headers.ContentType?.CharSet;
            if (!string.IsNullOrEmpty(charset))
                return charset;
            var content = await httpContent.ReadAsStringAsync();
            var match = Regex.Match(content, @"charset=(?<charset>.+?)""", RegexOptions.IgnoreCase);
            if (!match.Success)
                return charset;
            return match.Groups["charset"].Value;
        }
        #endregion


        #region HttpPostAsync
        /// <summary>
        /// POST 异步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postStream"></param>
        /// <param name="encoding"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, Dictionary<string, string> formData = null, Encoding encoding = null, int timeOut = 10000)
        {

            HttpClientHandler handler = new HttpClientHandler();

            HttpClient client = new HttpClient(handler);
            MemoryStream ms = new MemoryStream();
            formData.FillFormDataStream(ms);//填充formData
            HttpContent hc = new StreamContent(ms);


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            hc.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36");
            hc.Headers.Add("Timeout", timeOut.ToString());
            hc.Headers.Add("KeepAlive", "true");

            var r = await client.PostAsync(url, hc);
            byte[] tmp = await r.Content.ReadAsByteArrayAsync();

            return encoding.GetString(tmp);
        }

        #endregion

        /// <summary>
        /// 填充表单信息的Stream
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="stream"></param>
        public static void FillFormDataStream(this Dictionary<string, string> formData, Stream stream)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            stream.Write(formDataBytes, 0, formDataBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置
        }

        /// <summary>
        /// 组装QueryString的方法
        /// 参数之间用&连接，首位没有符号，如：a=1&b=2&c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string GetQueryString(this Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }
    }
}
