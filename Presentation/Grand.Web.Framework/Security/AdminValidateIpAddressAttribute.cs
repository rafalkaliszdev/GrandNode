﻿//using System;
///*using System.Web;*/
//
//using Grand.Core;
//using Grand.Core.Domain.Security;
//using Grand.Core.Infrastructure;
//using System.Linq;

//namespace Grand.Web.Framework.Security
//{
//    public class AdminValidateIpAddressAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            if (filterContext == null || filterContext.HttpContext == null)
//                return;

//            HttpRequestBase request = filterContext.HttpContext.Request;
//            if (request == null)
//                return;

//            //don't apply filter to child methods
//            if (filterContext.IsChildAction)
//                return;
//            bool ok = false;
//            var ipAddresses = EngineContextExperimental.Current.Resolve<SecuritySettings>().AdminAreaAllowedIpAddresses;
//            if (ipAddresses != null && ipAddresses.Any())
//            {
//                var webHelper = EngineContextExperimental.Current.Resolve<IWebHelper>();
//                foreach (string ip in ipAddresses)
//                    if (ip.Equals(webHelper.GetCurrentIpAddress(), StringComparison.OrdinalIgnoreCase))
//                    {
//                        ok = true;
//                        break;
//                    }
//            }
//            else
//            {
//                //no restrictions
//                ok = true;
//            }

//            if (!ok)
//            {
//                //ensure that it's not 'Access denied' page
//                var webHelper = EngineContextExperimental.Current.Resolve<IWebHelper>();
//                var thisPageUrl = webHelper.GetThisPageUrl(false);
//                if (!thisPageUrl.StartsWith(string.Format("{0}admin/security/accessdenied", webHelper.GetStoreLocation()), StringComparison.OrdinalIgnoreCase))
//                {
//                    //redirect to 'Access denied' page
//                    filterContext.Result = new RedirectResult(webHelper.GetStoreLocation() + "admin/security/accessdenied");
//                    //filterContext.Result = RedirectToAction("AccessDenied", "Security");
//                }
//            }
//        }
//    }
//}
