using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    //just a small help during transforming controllers into view components
    public class InitController : BasePublicController
    {
        public InitController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}