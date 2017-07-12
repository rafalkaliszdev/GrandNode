
using Grand.Web.Framework.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        ////[GrandHttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {

            
            return View();
        }
    }
}
