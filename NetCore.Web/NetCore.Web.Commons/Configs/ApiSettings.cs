using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Web.Commons.Configs
{
    public class ApiSettings
    {
        /// <summary>
        /// 授权地址
        /// </summary>
        public string AuthorizeUrl { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密码
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// API地址
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// api访问参数配置
        /// </summary>
        public ApiAccessConfig ApiAccessSettings { get; set; }

    }
}
