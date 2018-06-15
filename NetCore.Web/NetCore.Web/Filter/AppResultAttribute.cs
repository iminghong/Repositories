using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Filter
{
    public class AppResultAttribute : IResultFilter
    {
        private Stopwatch timer;
        public void OnResultExecuted(ResultExecutedContext context)
        {
            timer.Stop();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            timer = Stopwatch.StartNew();
        }
    }
}
