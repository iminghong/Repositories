using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Web.Commons.Configs
{
    public class RedisSettings
    {
        public string Connection { get; set; }

        public string DefaultDatabase { get; set; }

        public string InstanceName { get; set; }
    }
}
