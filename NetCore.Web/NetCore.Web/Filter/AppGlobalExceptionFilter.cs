using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetCore.Web.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetCore.Web.Filter
{
    public class AppGlobalExceptionFilter : IExceptionFilter
    {
        public readonly IHostingEnvironment _env;

        public AppGlobalExceptionFilter(IHostingEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            //添加日志
            LogHelpProvider.Error(context.Exception.TargetSite.ReflectedType, context.Exception);

            //返回错误信息
            var json = new ErrorResponse("web.Exception");
            if (_env.IsDevelopment()) {
                json.DeveloperMessage = context.Exception;
            }
            context.Result = new ApplicationErrorResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.ExceptionHandled = true;
        }
    }
    public class ApplicationErrorResult : ObjectResult
    {
        public ApplicationErrorResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }

    public class ErrorResponse
    {
        public ErrorResponse(string msg)
        {
            Message = msg;
        }
        public string Message { get; set; }
        public object DeveloperMessage { get; set; }
    }
}
