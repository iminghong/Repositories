using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetCore.Web.Commons;
using NetCore.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Web.Filter
{
    public class AppActionFilter : IActionFilter
    {
        private LogModel logmodel = new LogModel();
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var httpContext = context.HttpContext;
            var stopwach = httpContext.Items["StopwachKey"] as Stopwatch;
            stopwach.Stop();

            var controller = context.Controller as Controller;
            logmodel.ResultValue = JsonConvert.SerializeObject(context.Result);
            logmodel.InputTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var time = stopwach.Elapsed;
            logmodel.ExecTime = time.TotalSeconds.ToString();
            if (time.TotalSeconds > 5)
            {
                //添加日志
                LogHelpProvider.Info(context.ActionDescriptor, JsonConvert.SerializeObject(logmodel));
                //LogHelpProvider.Warn(context.ActionDescriptor, $"{context.ActionDescriptor.DisplayName}执行耗时:{time.ToString()}");
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

            //日志
            var controller = context.Controller as Controller;
            logmodel.Id = Guid.NewGuid().ToString();
            logmodel.Namespace = controller.GetType().Namespace;
            logmodel.ClassName = controller.GetType().Name;
            logmodel.MethodName = controller.HttpContext.Request.GetAbsoluteUri();//string.Join("/", controller.RouteData.Values.Values);
            logmodel.Parameter = JsonConvert.SerializeObject(controller.HttpContext.Request.Query);
            logmodel.LogType = "1";
            logmodel.Ip = controller.HttpContext.Connection.RemoteIpAddress.ToString();
            logmodel.Source = "web";

            var stopwach = new Stopwatch();
            stopwach.Start();
            context.HttpContext.Items.Add("StopwachKey", stopwach);
        }
    }

    //获取ip
    public static class HttpRequestExtensions
    {
        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }
    }
}
