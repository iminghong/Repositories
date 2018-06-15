using Microsoft.AspNetCore.Mvc.Filters;
using NetCore.Web.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Filter
{
    public class AppResultAttribute : IResultFilter
    {
        private Stopwatch stopwach;
        public void OnResultExecuted(ResultExecutedContext context)
        {
            stopwach.Stop();
            var time = stopwach.Elapsed;
            if (time.TotalSeconds > 5)
            {
                //添加日志
                LogHelpProvider.Warn(context.Exception.TargetSite.ReflectedType, $"{context.ActionDescriptor.DisplayName}执行耗时:{time.ToString()}");
            }
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            stopwach = Stopwatch.StartNew();
        }
    }
}
