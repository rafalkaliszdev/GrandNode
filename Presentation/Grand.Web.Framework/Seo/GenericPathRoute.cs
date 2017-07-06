//using System;
///*using System.Web;*/
///*using System.Web.Routing;*/
//using Grand.Core;
//using Grand.Core.Data;
//using Grand.Core.Infrastructure;
//using Grand.Services.Events;
//using Grand.Services.Seo;
//using Grand.Web.Framework.Localization;
//using System.Net;
//using Microsoft.AspNetCore.Routing;


//namespace Grand.Web.Framework.Seo
//{
//    /// <summary>
//    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
//    /// </summary>
//    public partial class GenericPathRoute //: LocalizedRoute
//    {
//        #region Constructors

//        /// <summary>
//        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern and handler class.
//        /// </summary>
//        /// <param name="url">The URL pattern for the route.</param>
//        /// <param name="routeHandler">The object that processes requests for the route.</param>
//        //public GenericPathRoute(string url, IRouteHandler routeHandler)
//        //    : base(url, routeHandler)
//        //{
//        //}

//        ///// <summary>
//        ///// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class and default parameter values.
//        ///// </summary>
//        ///// <param name="url">The URL pattern for the route.</param>
//        ///// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
//        ///// <param name="routeHandler">The object that processes requests for the route.</param>
//        //public GenericPathRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
//        //    : base(url, defaults, routeHandler)
//        //{
//        //}

//        ///// <summary>
//        ///// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values and constraints.
//        ///// </summary>
//        ///// <param name="url">The URL pattern for the route.</param>
//        ///// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
//        ///// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
//        ///// <param name="routeHandler">The object that processes requests for the route.</param>
//        //public GenericPathRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
//        //    : base(url, defaults, constraints, routeHandler)
//        //{
//        //}

//        ///// <summary>
//        ///// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values, 
//        ///// constraints,and custom values.
//        ///// </summary>
//        ///// <param name="url">The URL pattern for the route.</param>
//        ///// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
//        ///// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
//        ///// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. The route handler might need these values to process the request.</param>
//        ///// <param name="routeHandler">The object that processes requests for the route.</param>
//        //public GenericPathRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
//        //    : base(url, defaults, constraints, dataTokens, routeHandler)
//        //{
//        //}

//        //#endregion

//        //#region Methods

//        ///// <summary>
//        ///// Returns information about the requested route.
//        ///// </summary>
//        ///// <param name="httpContextAccessor">An object that encapsulates information about the HTTP request.</param>
//        ///// <returns>
//        ///// An object that contains the values from the route definition.
//        ///// </returns>
//        //public override RouteData GetRouteData(HttpContextBase httpContextAccessor)
//        //{
//        //    RouteData data = base.GetRouteData(httpContextAccessor);
//        //    if (data != null && DataSettingsHelper.DatabaseIsInstalled())
//        //    {
//        //        var urlRecordService = EngineContextExperimentalExperimental.Current.Resolve<IUrlRecordService>();
//        //        var slug = data.Values["generic_se_name"] as string;
//        //        //performance optimization.
//        //        //we load a cached verion here. it reduces number of SQL requests for each page load
//        //        var urlRecord = urlRecordService.GetBySlugCached(slug);
//        //        //comment the line above and uncomment the line below in order to disable this performance "workaround"
//        //        //var urlRecord = urlRecordService.GetBySlug(slug);
//        //        if (urlRecord == null)
//        //        {
//        //            //no URL record found
//        //            data.Values["controller"] = "Common";
//        //            data.Values["action"] = "PageNotFound";
//        //            return data;
//        //        }
//        //        //ensre that URL record is active
//        //        if (!urlRecord.IsActive)
//        //        {
//        //            //URL record is not active. let's find the latest one
//        //            var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
//        //            if (string.IsNullOrWhiteSpace(activeSlug))
//        //            {
//        //                //no active slug found
//        //                data.Values["controller"] = "Common";
//        //                data.Values["action"] = "PageNotFound";
//        //                return data;
//        //            }

//        //            //the active one is found
//        //            var webHelper = EngineContextExperimentalExperimental.Current.Resolve<IWebHelper>();
//        //            var response = httpContextAccessor.Response;
//        //            response.Status = "301 Moved Permanently";
//        //            response.RedirectLocation = string.Format("{0}{1}", webHelper.GetStoreLocation(), activeSlug);
//        //            response.End();
//        //            return null;
//        //        }

//        //        //ensure that the slug is the same for the current language
//        //        //otherwise, it can cause some issues when customers choose a new language but a slug stays the same
//        //        var workContext = EngineContextExperimentalExperimental.Current.Resolve<IWorkContext>();
//        //        var slugForCurrentLanguage = SeoExtensions.GetSeName(urlRecord.EntityId, urlRecord.EntityName, workContext.WorkingLanguage.Id);
//        //        if (!String.IsNullOrEmpty(slugForCurrentLanguage) &&
//        //            !slugForCurrentLanguage.Equals(slug, StringComparison.OrdinalIgnoreCase))
//        //        {
//        //            //we should make not null or "" validation above because some entities does not have SeName for standard (ID=0) language (e.g. news, blog posts)
//        //            var webHelper = EngineContextExperimentalExperimental.Current.Resolve<IWebHelper>();
//        //            var response = httpContextAccessor.Response;
//        //            //response.Status = "302 Found";
//        //            response.Status = "302 Moved Temporarily";
//        //            response.RedirectLocation = string.Format("{0}{1}", webHelper.GetStoreLocation(), slugForCurrentLanguage);
//        //            response.End();
//        //            return null;
//        //        }

//        //        //process URL
//        //        switch (urlRecord.EntityName.ToLowerInvariant())
//        //        {
//        //            case "product":
//        //                {
//        //                    data.Values["controller"] = "Product";
//        //                    data.Values["action"] = "ProductDetails";
//        //                    data.Values["productid"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            case "category":
//        //                {
//        //                    data.Values["controller"] = "Catalog";
//        //                    data.Values["action"] = "Category";
//        //                    data.Values["categoryid"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            case "manufacturer":
//        //                {
//        //                    data.Values["controller"] = "Catalog";
//        //                    data.Values["action"] = "Manufacturer";
//        //                    data.Values["manufacturerid"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            case "vendor":
//        //                {
//        //                    data.Values["controller"] = "Catalog";
//        //                    data.Values["action"] = "Vendor";
//        //                    data.Values["vendorid"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            case "newsitem":
//        //                {
//        //                    data.Values["controller"] = "News";
//        //                    data.Values["action"] = "NewsItem";
//        //                    data.Values["newsItemId"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            case "blogpost":
//        //                {
//        //                    data.Values["controller"] = "Blog";
//        //                    data.Values["action"] = "BlogPost";
//        //                    data.Values["blogPostId"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            case "topic":
//        //                {
//        //                    data.Values["controller"] = "Topic";
//        //                    data.Values["action"] = "TopicDetails";
//        //                    data.Values["topicId"] = urlRecord.EntityId;
//        //                    data.Values["SeName"] = urlRecord.Slug;
//        //                }
//        //                break;
//        //            default:
//        //                {
//        //                    //no record found

//        //                    //generate an event this way developers could insert their own types
//        //                    EngineContextExperimentalExperimental.Current.Resolve<IEventPublisher>()
//        //                        .Publish(new CustomUrlRecordEntityNameRequested(data, urlRecord));
//        //                }
//        //                break;
//        //        }
//        //    }
//        //    return data;
//        //}

//        #endregion
//    }
//}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Infrastructure;
using Grand.Services.Events;
using Grand.Services.Seo;
using Grand.Web.Framework.Localization;

namespace Grand.Web.Framework.Seo
{
    /// <summary>
    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
    /// </summary>
    public class GenericPathRoute : LocalizedRoute
    {
        #region Fields

        private readonly IRouter _target;

        #endregion

        #region Ctor

        public GenericPathRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults,
            IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override Task RouteAsync(RouteContext context)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return Task.CompletedTask;

            //get current route data
            var urlRecordService = EngineContextExperimental.Current.Resolve<IUrlRecordService>();

            //get slug from path
            var currentRouteData = new RouteData(context.RouteData);

            //tbh
            string slug = string.Empty;
#if NET451
            slug = currentRouteData.Values["generic_se_name"] as string;
#endif
#if NETCOREAPP1_1
            //temporary solution until we can get currentRouteData.Values
            string path = context.HttpContext.Request.Path.Value;
            if (!string.IsNullOrEmpty(path) && path[0] == '/')
                path = path.Substring(1);
            var pathParts = path.Split('/');
            slug = pathParts.Length > 1 ? pathParts[0] : path;
#endif

            //performance optimization
            //we load a cached verion here. It reduces number of SQL requests for each page load
            var urlRecord = urlRecordService.GetBySlugCached(slug);

            //comment the line above and uncomment the line below in order to disable this performance "workaround"
            //var urlRecord = urlRecordService.GetBySlug(slug);

            //no URL record found
            if (urlRecord == null)
                return Task.CompletedTask;

            if (!urlRecord.IsActive)
            {
                //URL record is not active. let's find the latest one
                var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);

                //no active slug found
                if (string.IsNullOrEmpty(activeSlug))
                    return Task.CompletedTask;

                //the active one is found
                var webHelper = EngineContextExperimental.Current.Resolve<IWebHelper>();
                var location = string.Format("{0}{1}", webHelper.GetStoreLocation(), activeSlug);
                context.HttpContext.Response.Redirect(location, true);
                return Task.CompletedTask;
            }

            //ensure that the slug is the same for the current language
            //otherwise, it can cause some issues when customers choose a new language but a slug stays the same
            var workContext = EngineContextExperimental.Current.Resolve<IWorkContext>();
            var slugForCurrentLanguage = SeoExtensions.GetSeName(urlRecord.EntityId, urlRecord.EntityName, workContext.WorkingLanguage.Id);
            if (!string.IsNullOrEmpty(slugForCurrentLanguage) && !slugForCurrentLanguage.Equals(slug, StringComparison.OrdinalIgnoreCase))
            {
                //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)
                var webHelper = EngineContextExperimental.Current.Resolve<IWebHelper>();
                var location = string.Format("{0}{1}", webHelper.GetStoreLocation(), slugForCurrentLanguage);
                context.HttpContext.Response.Redirect(location, false);
                return Task.CompletedTask;
            }

            //process URL
            switch (urlRecord.EntityName.ToLowerInvariant())
            {
                case "product":
                    currentRouteData.Values["controller"] = "Product";
                    currentRouteData.Values["action"] = "ProductDetails";
                    currentRouteData.Values["productid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "category":
                    currentRouteData.Values["controller"] = "Catalog";
                    currentRouteData.Values["action"] = "Category";
                    currentRouteData.Values["categoryid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "manufacturer":
                    currentRouteData.Values["controller"] = "Catalog";
                    currentRouteData.Values["action"] = "Manufacturer";
                    currentRouteData.Values["manufacturerid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "vendor":
                    currentRouteData.Values["controller"] = "Catalog";
                    currentRouteData.Values["action"] = "Vendor";
                    currentRouteData.Values["vendorid"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "newsitem":
                    currentRouteData.Values["controller"] = "News";
                    currentRouteData.Values["action"] = "NewsItem";
                    currentRouteData.Values["newsItemId"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "blogpost":
                    currentRouteData.Values["controller"] = "Blog";
                    currentRouteData.Values["action"] = "BlogPost";
                    currentRouteData.Values["blogPostId"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                case "topic":
                    currentRouteData.Values["controller"] = "Topic";
                    currentRouteData.Values["action"] = "TopicDetails";
                    currentRouteData.Values["topicId"] = urlRecord.EntityId;
                    currentRouteData.Values["SeName"] = urlRecord.Slug;
                    break;
                default:
                    //no record found
                    //thus generate an event this way developers could insert their own types
                    EngineContextExperimental.Current.Resolve<IEventPublisher>().Publish(new CustomUrlRecordEntityNameRequested(currentRouteData, urlRecord));
                    break;
            }
            context.RouteData = currentRouteData;

            //route request
            return _target.RouteAsync(context);
        }

#endregion
    }
}