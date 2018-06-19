using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetCore.Web.Commons;
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
                //添加日志
                LogHelpProvider.Warn(context.ActionDescriptor, $"{context.ActionDescriptor.DisplayName}执行耗时:{time.ToString()}");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //验证数据
            if (!context.ModelState.IsValid)
            {
                var modelState = context.ModelState.FirstOrDefault(f => f.Value.Errors.Any());
                string errorMsg = modelState.Value.Errors.First().ErrorMessage;
                throw new Exception(errorMsg);
            }

            var controller = context.Controller as Controller;
            //controller.HttpContext.Request.Query

            var stopwach = new Stopwatch();
            stopwach.Start();
            context.HttpContext.Items.Add("StopwachKey", stopwach);
        }
    }
}
