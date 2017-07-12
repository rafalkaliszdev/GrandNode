//using System;
//
///*using System.Web.Routing;*/
//using Microsoft.AspNetCore.Routing;
//using Microsoft.AspNetCore.Mvc;

//namespace Grand.Web.Framework.Seo
//{
//    public static class GenericPathRouteExtensions
//    {
//        //Override for localized route
//        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url)
//        {
//            return MapGenericPathRoute(routes, name, url, null /* defaults */, (object)null /* constraints */);
//        }
//        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults)
//        {
//            return MapGenericPathRoute(routes, name, url, defaults, (object)null /* constraints */);
//        }
//        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults, object constraints)
//        {
//            return MapGenericPathRoute(routes, name, url, defaults, constraints, null /* namespaces */);
//        }
//        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, string[] namespaces)
//        {
//            return MapGenericPathRoute(routes, name, url, null /* defaults */, null /* constraints */, namespaces);
//        }
//        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
//        {
//            return MapGenericPathRoute(routes, name, url, defaults, null /* constraints */, namespaces);
//        }
//        public static Route MapGenericPathRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
//        {
//            if (routes == null)
//            {
//                throw new ArgumentNullException("routes");
//            }
//            if (url == null)
//            {
//                throw new ArgumentNullException("url");
//            }

//            //var route = new GenericPathRoute(url, new MvcRouteHandler())
//            //{
//            //    Defaults = new RouteValueDictionary(defaults),
//            //    Constraints = new RouteValueDictionary(constraints),
//            //    DataTokens = new RouteValueDictionary()
//            //};

//            //if ((namespaces != null) && (namespaces.Length > 0))
//            //{
//            //    route.DataTokens["Namespaces"] = namespaces;
//            //}

//            //routes.Add(name, route);

//            return null;// route;
//        }
//    }
//}



using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Grand.Core.Infrastructure;

namespace Grand.Web.Framework.Seo
{
    /// <summary>
    /// Represents extensions of GenericPathRoute
    /// </summary>
    public static class GenericPathRouteExtensions
    {
        /// <summary>
        /// Adds a route to the route builder with the specified name and template
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapGenericPathRoute(this IRouteBuilder routeBuilder, string name, string template)
        {
            return MapGenericPathRoute(routeBuilder, name, template, defaults: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, and default values
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults">An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapGenericPathRoute(this IRouteBuilder routeBuilder, string name, string template, object defaults)
        {
            return MapGenericPathRoute(routeBuilder, name, template, defaults, constraints: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, and constraints.
        /// </summary>
        /// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults"> An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <param name="constraints">An object that contains constraints for the route. 
        /// The object's properties represent the names and values of the constraints</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapGenericPathRoute(this IRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints)
        {
            return MapGenericPathRoute(routeBuilder, name, template, defaults, constraints, dataTokens: null);
        }

        /// <summary>
        /// Adds a route to the route builder with the specified name, template, default values, constraints anddata tokens.
        /// </summary>
        //// <param name="routeBuilder">The route builder to add the route to</param>
        /// <param name="name">The name of the route</param>
        /// <param name="template">The URL pattern of the route</param>
        /// <param name="defaults"> An object that contains default values for route parameters. 
        /// The object's properties represent the names and values of the default values</param>
        /// <param name="constraints">An object that contains constraints for the route. 
        /// The object's properties represent the names and values of the constraints</param>
        /// <param name="dataTokens">An object that contains data tokens for the route. 
        /// The object's properties represent the names and values of the data tokens</param>
        /// <returns>Route builder</returns>
        public static IRouteBuilder MapGenericPathRoute(this IRouteBuilder routeBuilder,
            string name, string template, object defaults, object constraints, object dataTokens)
        {
            if (routeBuilder.DefaultHandler == null)
                throw new ArgumentNullException(nameof(routeBuilder));

            //get registered InlineConstraintResolver
            var inlineConstraintResolver = routeBuilder
                .ServiceProvider
                .GetRequiredService<IInlineConstraintResolver>();

            //create new generic route
            routeBuilder.Routes.Add(new GenericPathRoute(
                routeBuilder.DefaultHandler,
                name,
                template,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new RouteValueDictionary(dataTokens),
                inlineConstraintResolver));

            return routeBuilder;
        }
    }
}