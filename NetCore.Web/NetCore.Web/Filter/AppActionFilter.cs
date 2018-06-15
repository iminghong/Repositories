using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Filter
{
    public class AppActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var httpContext = context.HttpContext;
            var stopwach = httpContext.Items["StopwachKey"] as Stopwatch;
            stopwach.Stop();
            var time = stopwach.Elapsed;

            if (time.TotalSeconds > 5)
            {
                var factory = (ILoggerFactory)context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) ;
                var logger = factory.CreateLogger<ActionExecutedContext>();
                logger.LogWarning($"{context.ActionDescriptor.DisplayName}执行耗时:{time.ToString()}");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var modelState = context.ModelState.FirstOrDefault(f => f.Value.Errors.Any());
                string errorMsg = modelState.Value.Errors.First().ErrorMessage;
                throw new Exception(errorMsg);
            }

            var stopwach = new Stopwatch();
            stopwach.Start();
            context.HttpContext.Items.Add("StopwachKey", stopwach);
        }
    }
}
