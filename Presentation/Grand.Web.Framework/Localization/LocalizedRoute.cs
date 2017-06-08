/*using System.Web;*/
/*using System.Web.Routing;*/
using Grand.Core.Data;
using Grand.Core.Domain.Localization;
using Grand.Core.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace Grand.Web.Framework.Localization
{
    /// <summary>
    /// Provides properties and methods for defining a localized route, and for getting information about the localized route.
    /// </summary>
    public class LocalizedRoute : Route
    {
        #region Fields

        private bool? _seoFriendlyUrlsForLanguagesEnabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern and handler class.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LocalizedRoute(IRouter target, string /*url*/routeTemplate, IInlineConstraintResolver inlineConstraintResolver)//IRouteHandler routeHandler)
            : base(target, routeTemplate, inlineConstraintResolver) //(url, routeHandler)
        {
            //so it just is lling called during startup without registering ?
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class and default parameter values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        //public LocalizedRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
        //    : base(url, defaults, routeHandler)
        //{
        //}

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        //public LocalizedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
        //    : base(url, defaults, constraints, routeHandler)
        //{
        //}

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values, 
        /// constraints,and custom values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. The route handler might need these values to process the request.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        //public LocalizedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
        //    : base(url, defaults, constraints, dataTokens, routeHandler)
        //{
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Returns information about the requested route.
        /// </summary>
        /// <param name="httpContextAccessor">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        //public override RouteData GetRouteData(HttpContextBase httpContextAccessor)
        //{
        //    if (DataSettingsHelper.DatabaseIsInstalled() && this.SeoFriendlyUrlsForLanguagesEnabled)
        //    {
        //        string virtualPath = httpContextAccessor.Request.AppRelativeCurrentExecutionFilePath;
        //        string applicationPath = httpContextAccessor.Request.ApplicationPath;
        //        if (virtualPath.IsLocalizedUrl(applicationPath, false))
        //        {
        //            //In ASP.NET Development Server, an URL like "http://localhost/Blog.aspx/Categories/BabyFrog" will return 
        //            //"~/Blog.aspx/Categories/BabyFrog" as AppRelativeCurrentExecutionFilePath.
        //            //However, in II6, the AppRelativeCurrentExecutionFilePath is "~/Blog.aspx"
        //            //It seems that IIS6 think we're process Blog.aspx page.
        //            //So, I'll use RawUrl to re-create an AppRelativeCurrentExecutionFilePath like ASP.NET Development Server.

        //            //Question: should we do path rewriting right here?
        //            string rawUrl = httpContextAccessor.Request.RawUrl;
        //            var newVirtualPath = rawUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
        //            if (string.IsNullOrEmpty(newVirtualPath))
        //                newVirtualPath = "/";
        //            newVirtualPath = newVirtualPath.RemoveApplicationPathFromRawUrl(applicationPath);
        //            newVirtualPath = "~" + newVirtualPath;
        //            httpContextAccessor.RewritePath(newVirtualPath, true);
        //        }
        //    }
        //    RouteData data = base.GetRouteData(httpContextAccessor);
        //    return data;
        //}

        /// <summary>
        /// Returns information about the URL that is associated with the route.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param>
        /// <param name="values">An object that contains the parameters for a route.</param>
        /// <returns>
        /// An object that contains information about the URL that is associated with the route.
        /// </returns>
        //public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        //{
        //    VirtualPathData data = base.GetVirtualPath(requestContext, values);

        //    if (data != null && DataSettingsHelper.DatabaseIsInstalled() && this.SeoFriendlyUrlsForLanguagesEnabled)
        //    {
        //        string rawUrl = requestContext.HttpContext.Request.RawUrl;
        //        string applicationPath = requestContext.HttpContext.Request.ApplicationPath;
        //        if (rawUrl.IsLocalizedUrl(applicationPath, true))
        //        {
        //            data.VirtualPath = string.Concat(rawUrl.GetLanguageSeoCodeFromUrl(applicationPath, true), "/",
        //                data.VirtualPath);
        //        }
        //    }
        //    return data;
        //}

        public virtual void ClearSeoFriendlyUrlsCachedValue()
        {
            _seoFriendlyUrlsForLanguagesEnabled = null;
        }

        #endregion

        #region Properties

        protected bool SeoFriendlyUrlsForLanguagesEnabled
        {
            get
            {
                if (!_seoFriendlyUrlsForLanguagesEnabled.HasValue)
                    _seoFriendlyUrlsForLanguagesEnabled = EngineContextExperimental.Current.Resolve<LocalizationSettings>().SeoFriendlyUrlsForLanguagesEnabled;

                return _seoFriendlyUrlsForLanguagesEnabled.Value;
            }
        }

        #endregion
    }
}