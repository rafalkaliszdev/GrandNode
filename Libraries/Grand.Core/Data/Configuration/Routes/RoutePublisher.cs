using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Web.Routing;*/
using Grand.Core.Infrastructure;
using Grand.Core.Plugins;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;

//namespace Grand.Web.Framework.Mvc.Routes
namespace Grand.Core.Configuration.Routes
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public class RoutePublisher : IRoutePublisher
    {
        protected readonly ITypeFinder typeFinder;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder"></param>
        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        /// <summary>
        /// Find a plugin descriptor by some type which is located into its assembly
        /// </summary>
        /// <param name="providerType">Provider type</param>
        /// <returns>Plugin descriptor</returns>
        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");

            foreach (var plugin in PluginManager.ReferencedPlugins)
            {
                if (plugin.ReferencedAssembly == null)
                    continue;

                //tbh
                if (plugin.ReferencedAssembly.FullName == providerType.AssemblyQualifiedName/*.Assembly.FullName*/)
                    return plugin;
            }

            return null;
        }



        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        public virtual void/*List<NameTemplateDefaults>*/ RegisterRoutes(IApplicationBuilder app)
        {
            var routeProviderTypes = typeFinder.FindClassesOfType<IRouteProvider>();
            var routeProviders = new List<IRouteProvider>();
            foreach (var providerType in routeProviderTypes)
            {
                //tbh wont need plugins for now
                //Ignore not installed plugins
                //var plugin = FindPlugin(providerType);
                //if (plugin != null && !plugin.Installed)
                //    continue;

                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                routeProviders.Add(provider);
            }
            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();


            
            app.UseMvc(routeBuilder =>
            {
                //should be only 2 elements in collection for now
                foreach(var routeProvider in routeProviders)
                {
                    routeProvider.RegisterRoutes(routeBuilder);
                }
            });























            //2017_07_03 previous and working

            //var routesToBeRegistered = new List<NameTemplateDefaults>();
            //foreach (var routeProvider in routeProviders)
            //{
            //    routesToBeRegistered.Concat(routeProvider.CollectRoutes(/*app*/routesToBeRegistered));
            //    //collectedRoutes.AddRange(dqwqqw.CollectRoutes(/*app*/collectedRoutes));
            //}

            //this is default route
            //apparently it isnt needed, but can be left
            //routesToBeRegistered/*routes*/.Add(new NameTemplateDefaults(
            //    name: "default",
            //    template: "{Home}/{Index}/{id?}", //"{controller=Home}/{action=Index}/{id?}",
            //    defaults: new { controller = "Home", action = "Index" }
            //    ));

            ////2017_06_06
            ////ad default route
            //routesToBeRegistered.Add(new NameTemplateDefaults(
            //    "Default", // Route name
            //    "{controller}/{action}/{id?}", // URL with parameters
            //     new { controller = "Home", action = "Index" }
            //    ));

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            //    foreach (var route in routesToBeRegistered)
            //    {
            //        routes.MapRoute(
            //            name: route.Name,
            //            template: route.Template,
            //            defaults: route.Defaults
            //            );
            //    }
            //});
        }

    }
    public class NameTemplateDefaults
    {
        public NameTemplateDefaults(string name, string template, object defaults)
        {
            this.Name = name;
            this.Template = template;
            this.Defaults = defaults;
        }

        public string Name { get; set; }
        public string Template { get; set; }
        public object Defaults { get; set; }
    }
}
