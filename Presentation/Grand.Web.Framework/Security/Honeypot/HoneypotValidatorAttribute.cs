﻿//using System;
//
//using Grand.Core;
//using Grand.Core.Domain.Security;
//using Grand.Core.Infrastructure;
//using Grand.Services.Logging;

//namespace Grand.Web.Framework.Security.Honeypot
//{
//    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
//    public class HoneypotValidatorAttribute : FilterAttribute, IAuthorizationFilter
//    {
//        public void OnAuthorization(AuthorizationContext filterContext)
//        {
//            if (filterContext == null)
//                throw new ArgumentNullException("filterContext");

//            var securitySettings = EngineContextExperimental.Current.Resolve<SecuritySettings>();
//            if (securitySettings.HoneypotEnabled)
//            {
//                string inputValue = filterContext.HttpContext.Request.Form[securitySettings.HoneypotInputName];

//                var isBot = !String.IsNullOrWhiteSpace(inputValue);
//                if (isBot)
//                {
//                    var logger = EngineContextExperimental.Current.Resolve<ILogger>();
//                    logger.Warning("A bot detected. Honeypot.");

//                    //filterContext.Result = new UnauthorizedResult();
//                    var webHelper = EngineContextExperimental.Current.Resolve<IWebHelper>();
//                    string url = webHelper.GetThisPageUrl(true);
//                    filterContext.Result = new RedirectResult(url);
//                }
//            }
//        }
//    }
//}
