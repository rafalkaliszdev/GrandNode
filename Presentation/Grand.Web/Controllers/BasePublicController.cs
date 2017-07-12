
//using System.Web.Routing;
using Grand.Core.Infrastructure;
using Grand.Web.Framework;
using Grand.Web.Framework.Controllers;
using Grand.Web.Framework.Security;
//using Grand.Web.Framework.Seo;//these attributes
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    //[CheckAffiliate]
    //[StoreClosed]
    [PublicStoreAllowNavigation]
    //[LanguageSeoCode]
    ////[GrandHttpsRequirement(SslRequirement.NoMatter)]
    //[WwwRequirement]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual IActionResult InvokeHttp404()
        {

            //tbh


            // Call target Controller and pass the routeData.
            //IController errorController = EngineContextExperimental.Current.Resolve<CommonController>();

            //var routeData = new RouteData();
            //routeData.Values.Add("controller", "Common");
            //routeData.Values.Add("action", "PageNotFound");

            //errorController.Execute(new RequestContext(this.HttpContext, routeData));

            return new EmptyResult();
        }

    }
}
