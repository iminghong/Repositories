using Microsoft.AspNetCore.Mvc;
using NetCore.Web.Commons;
using System;

namespace NetCore.Web.Controllers
{
    public class DefaultController: BaseController
    {
        public IActionResult Index()
        {
            LogHelpProvider.Debug(this, "default controller");
            return Content("11111");
        }
    }
}
