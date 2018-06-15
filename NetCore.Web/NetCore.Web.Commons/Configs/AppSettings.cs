using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Web.Commons.Configs
{
    public class AppSettings
    {
        /// <summary>
        /// 授权地址
        /// </summary>
        public string AuthorizeUrl { get; set; }

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
