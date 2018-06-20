using Microsoft.AspNetCore.Mvc;
using NetCore.Web.Commons;
using System;
using System.Threading.Tasks;

namespace NetCore.Web.Controllers
{
    public class DefaultController: BaseController
    {
        public async Task<IActionResult> Index()
        {
            TokenInfo tokeninfo= await HttpRequestProvider.GetTokenClaims("appid", "appPwd");


            LogHelpProvider.Debug(this, "default controller");
            return Content("11111");
        }
    }
}
