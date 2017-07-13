using System;
using System.Collections.Generic;
using System.IO;
//using System.Web.Mvc;
using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Core.Infrastructure;
using Grand.Services.Common;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Stores;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.UI;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Grand.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller
    /// </summary>
    //[StoreIpAddress]
    //[CustomerLastActivity]
    //[StoreLastVisitedPage]
    //[ValidatePassword]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <returns>Result</returns>
        [NonAction]
        public virtual string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <returns>Result</returns>
        [NonAction]
        public virtual string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        [NonAction]
        public virtual string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        [NonAction]
        public virtual string RenderPartialViewToString(string viewName, object model)
        {
            //get Razor view engine
            var razorViewEngine = EngineContextExperimental.Current.Resolve<IRazorViewEngine>();

            //create action context
            var actionContext = new ActionContext(this.HttpContext, this.RouteData, this.ControllerContext.ActionDescriptor, this.ModelState);

            //set view name as action name in case if not passed
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.ActionDescriptor.ActionName;

            //set model
            ViewData.Model = model;
            var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
            if (viewResult.View == null)
                return string.Format(($"{viewName} view was not found"));
                //throw new ArgumentNullException($"{viewName} view was not found");

            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(actionContext, viewResult.View, ViewData, TempData, stringWriter, new HtmlHelperOptions());

                var t = viewResult.View.RenderAsync(viewContext);
                t.Wait();
                return stringWriter.GetStringBuilder().ToString();
            }





















            string dupa = RenderToStringAsync(viewName, model).Result;
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/


            //tbh
            //if (string.IsNullOrEmpty(viewName))
            //    viewName = this.ControllerContext.RouteData.GetRequiredString("action");

            //this.ViewData.Model = model;

            //using (var sw = new StringWriter())
            //{
            //    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
            //    var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
            //    viewResult.View.Render(viewContext, sw);

            //    return sw.GetStringBuilder().ToString();
            //}
            return "RenderPartialViewToString()";
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {

            var _razorViewEngine = EngineContextExperimental.Current.Resolve<IRazorViewEngine>();
            var _tempDataProvider = EngineContextExperimental.Current.Resolve<ITempDataProvider>();
            var _serviceProvider = EngineContextExperimental.Current.Resolve<IServiceProvider>();


            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            //
            //var actionContext1 = new ActionContext(this.ControllerContext.ActionDescriptor.ActionName, new RouteData(), new ActionDescriptor());
            var actionContext2 = new ActionContext(ControllerContext.HttpContext, this.RouteData, this.ControllerContext.ActionDescriptor);
            
            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext2, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }



















        /// <summary>
        /// Get active store scope (for multi-store configuration mode)
        /// </summary>
        /// <param name="storeService">Store service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Store ID; 0 if we are in a shared mode</returns>
        [NonAction]
        public virtual string GetActiveStoreScopeConfiguration(IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return "";


            var storeId = workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);
            return store != null ? store.Id : "";
        }


        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exc">Exception</param>
        [NonAction]
        protected void LogException(Exception exc)
        {
            var workContext = EngineContextExperimental.Current.Resolve<IWorkContext>();
            var logger = EngineContextExperimental.Current.Resolve<ILogger>();

            var customer = workContext.CurrentCustomer;
            logger.Error(exc.Message, exc, customer);
        }
        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        [NonAction]
        protected virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        [NonAction]
        protected virtual void ErrorNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        [NonAction]
        protected virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
        {
            if (logException)
                LogException(exception);
            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }
        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        [NonAction]
        protected virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("Grand.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>)TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }

        [NonAction]
        protected virtual void DisplayEditLink(string editPageUrl)
        {
            //We cannot use ViewData because it works only for the current controller (and we pass and then render "Edit" link data in distinct controllers)
            //that's why we use IPageHeadBuilder

            //tbh

            //var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            //pageHeadBuilder.AddEditPageUrl(editPageUrl);
        }


        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        [NonAction]
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            AddLocales(languageService, locales, null);
        }
        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        [NonAction]
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, string> configure) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            foreach (var language in languageService.GetAllLanguages(true))
            {
                var locale = Activator.CreateInstance<TLocalizedModelLocal>();
                locale.LanguageId = language.Id;
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }
        }

    }
}
