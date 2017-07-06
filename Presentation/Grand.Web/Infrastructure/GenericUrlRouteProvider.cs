//using System.Web.Routing;
using Grand.Web.Framework.Localization;
//using Grand.Web.Framework.Mvc.Routes;
//using Grand.Web.Framework.Seo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

using Grand.Core.Configuration.Routes;
using System;
using Grand.Web.Framework.Seo;

namespace Grand.Web.Infrastructure
{
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                                       "{generic_se_name}",
                                       new { controller = "Common", action = "GenericUrl" });
            //new[] { "Grand.Web.Controllers" });

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routes.MapLocalizedRoute("Product",
                                     "{SeName}",
                                     new { controller = "Product", action = "ProductDetails" });
            //new[] { "Grand.Web.Controllers" });

            routes.MapLocalizedRoute("Category",
                            "{SeName}",
                            new { controller = "Catalog", action = "Category" });
            //new[] { "Grand.Web.Controllers" });

            routes.MapLocalizedRoute("Manufacturer",
                            "{SeName}",
                            new { controller = "Catalog", action = "Manufacturer" });
            //new[] { "Grand.Web.Controllers" });

            routes.MapLocalizedRoute("Vendor",
                            "{SeName}",
                            new { controller = "Catalog", action = "Vendor" });
            //new[] { "Grand.Web.Controllers" });

            routes.MapLocalizedRoute("NewsItem",
                            "{SeName}",
                            new { controller = "News", action = "NewsItem" });
            //new[] { "Grand.Web.Controllers" });

            routes.MapLocalizedRoute("BlogPost",
                            "{SeName}",
                            new { controller = "Blog", action = "BlogPost" });
            //new[] { "Grand.Web.Controllers" });

            routes.MapLocalizedRoute("Topic",
                            "{SeName}",
                            new { controller = "Topic", action = "TopicDetails" });
            //new[] { "Grand.Web.Controllers" });



            //the last route. it's used when none of registered routes could be used for the current request
            //but it this case we cannot process non-registered routes (/controller/action)
            //routes.MapLocalizedRoute(
            //    "PageNotFound-Wildchar",
            //    "{*url}",
            //    new { controller = "Common", action = "PageNotFound" });
            //    //new[] { "Grand.Web.Controllers" });
        }

        public List</*Grand.Web.Framework.Mvc.Routes.*/NameTemplateDefaults> CollectRoutes/*RegisterRoutes*/(List<NameTemplateDefaults> routes/*IApplicationBuilder app*/)
        {

            //IRouter dadas = new RouterMiddleware();
            //IRouteBuilder routes = new RouteBuilder(app,);





            routes.Add(new NameTemplateDefaults(
                "Category",
                "{SeName}",
                new { controller = "Catalog", action = "Category" }));
            /*new[] { "Grand.Web.Controllers" }));*/








            //below routes are good and working, but i dont need them now, and they may obscure what im doing now
            return routes;








            //generic URLs
            routes.Add(new NameTemplateDefaults(
                "GenericUrl",
                "{generic_se_name}",
                new { controller = "Common", action = "GenericUrl" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routes.Add(new NameTemplateDefaults(
                "Product",
                "{SeName}",
                new { controller = "Product", action = "ProductDetails" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            routes.Add(new NameTemplateDefaults(
                "Category",
                "{SeName}",
                new { controller = "Catalog", action = "Category" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            routes.Add(new NameTemplateDefaults(
                "Manufacturer",
                "{SeName}",
                new { controller = "Catalog", action = "Manufacturer" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            routes.Add(new NameTemplateDefaults(
                "Vendor",
                "{SeName}",
                new { controller = "Catalog", action = "Vendor" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            routes.Add(new NameTemplateDefaults(
                "NewsItem",
                "{SeName}",
                new { controller = "News", action = "NewsItem" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            routes.Add(new NameTemplateDefaults(
                "BlogPost",
                "{SeName}",
                new { controller = "Blog", action = "BlogPost" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            routes.Add(new NameTemplateDefaults(
                "Topic",
                "{SeName}",
                new { controller = "Topic", action = "TopicDetails" }));
            /*new[] { "Grand.Web.Controllers" }));*/



            //the last route.it's used when none of registered routes could be used for the current request
            //but it this case we cannot process non-registered routes(/ controller / action)
            routes.Add(new NameTemplateDefaults(
                "PageNotFound-Wildchar",
                "{*url}",
                new { controller = "Common", action = "PageNotFound" }));
            /*new[] { "Grand.Web.Controllers" }));*/


            return routes;


        }

        public int Priority
        {
            get
            {
                //it should be the last route
                //we do not set it to -int.MaxValue so it could be overridden (if required)
                return -1000000;
            }
        }
    }
}
