using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Web.Commons
{
    public class LogRepository
    {
        public static ILoggerRepository Repository { get; set; }
    }
}
