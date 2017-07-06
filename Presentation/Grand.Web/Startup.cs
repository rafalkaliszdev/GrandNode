//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
////using Grand.Web.Framework.Mvc.Routes;

//using Autofac;
//using Autofac.Extensions.DependencyInjection;
////using Microsoft.Net;
//using Grand.Core.Infrastructure;
//using Grand.Core.Infrastructure.DependencyManagement;
//using Grand.Core.Configuration;
//using Grand.Core.Data;
////using System;
////using System.Collections.Generic;
////using System.Linq;
/////*using System.Web.Mvc;*/
////using Autofac;
////using Autofac.Integration.Mvc;
////using AutoMapper;
////using Grand.Core.Configuration;
////using Grand.Core.Infrastructure.DependencyManagement;
////using Grand.Core.Infrastructure.Mapper;

//using System;
//using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Razor;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json.Serialization;


//using Microsoft.AspNetCore.Mvc.Razor;

//using Microsoft.AspNetCore.Mvc.ViewEngines;


//using Microsoft.Extensions.DependencyInjection;
//using Grand.Web.Framework.Themes;
//using Grand.Core.Configuration.Routes;

//namespace Grand.Web
//{
//    public class Startup
//    {

//        public IContainer _applicationContainer { get; private set; }


//        public IServiceProvider ConfigureServices(IServiceCollection services)
//        {
//            //no idea what it is and for what it is
//            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

//            //rather not used
//            //MvcHandler.DisableMvcResponseHeader = true;





//            IContainer applicationContainer = null;
//            RoutePublisher routePublisher = null;
//            AutofacServiceProvider autofacServiceProvider = null;
//            var asd = EngineContextExperimental.Initialize(false, services, /*out*/ applicationContainer, /*out *//*routePublisher,*/ /*out AutofacServiceProvider */autofacServiceProvider);

//            //applicationContainer = asd.Item1;
//            //_routePublisher = qqqqqalanalna.Item2;
//            //autofacServiceProvider = asd.Item2;








//            //bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
//            //if (databaseInstalled)
//            //{
//            //exp looks like deprecated
//            ////remove all view engines
//            //ViewEngines.Engines.Clear();
//            ////except the themeable razor view engine we use
//            //ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
//            //}

//            //exp looks like deprecated
//            //Add some functionality on top of the default ModelMetadataProvider
//            //ModelMetadataProviders.Current = new NopMetadataProvider();

//            //exp looks like deprecated
//            //Registering some regular mvc stuff
//            //AreaRegistration.RegisterAllAreas();

//            //RegisterRoutes() moved to ..RegisterRoutes() below



//            //looks like there will be something different than RouteTable..






//            //Themeable Razor View Engine
//            services.Configure<RazorViewEngineOptions>(options =>
//            {
//                options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
//            });

//            services.AddMvc();
            
//            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//            //services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

//            //new
//            //return new AutofacServiceProvider(applicationContainer);
//            //return autofacServiceProvider;
//            //previous
//            return this.RegisterDependencies(services, new GrandConfig());
//        }



//        public AutofacServiceProvider RegisterDependencies(IServiceCollection services, GrandConfig config)
//        {

//            //2017_05_18
//            //not cut, just copied into GrandEngine and commented

//            //code below should be inside GrandEngine.RegisterDependencies() but i need autofac object here


//            var typeFinder = new WebAppTypeFinder();
//            var builder = new ContainerBuilder();

//            builder.RegisterInstance(config).As<GrandConfig>().SingleInstance();
//            builder.RegisterInstance(Singleton<IEngine>.Instance/*this*/).As<IEngine>().SingleInstance();
//            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

//            //it needs to be here
//            //builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
//            _routePublisher = new RoutePublisher(typeFinder);
//            builder.RegisterInstance(_routePublisher).As<IRoutePublisher>().SingleInstance();

//            //register dependencies provided by other assemblies
//            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
//            var drInstances = new List<IDependencyRegistrar>();
//            foreach (var drType in drTypes)
//                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

//            //sort
//            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
//            foreach (var dependencyRegistrar in drInstances)
//                dependencyRegistrar.Register(builder, typeFinder, config);

//            builder.Populate(services);
//            this._applicationContainer = builder.Build();

//            //here should be EngineContextExperimental
//            return new AutofacServiceProvider(this._applicationContainer);











//            //previous way from gn_origin
//            ////set dependency resolver
//            ////DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
//        }


//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
//        {
//            loggerFactory.AddConsole();

//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }



//            //RegisterRoutes(app);
//            _routePublisher.RegisterRoutes(app);


//            app.Run(async (context) =>
//            {
//                if (context.Request.Path == "/favicon.ico")
//                {
//                    await context.Response.SendFileAsync("favicon.ico");
//                }
//                else
//                {
//                    //await context.Response.SendFileAsync("favicon.ico");

//                    await context.Response.SendFileAsync("qqqqq.txt");
//                    await context.Response.WriteAsync("Hello World!");
//                }

//            });
//        }

//        public static RoutePublisher _routePublisher;

//        //public static void RegisterRoutes(/*RouteCollection routes*/IApplicationBuilder app)
//        //{

//            //var qqqqqs = new List<NameTemplateDefaults>();

//            //now gather these routes
//            //ex
//            //var routePublisher = EngineContextExperimental.Current.Resolve<IRoutePublisher>();
//            //routePublisher.RegisterRoutes(app);





//            //_routePublisher.RegisterRoutes(app);





//            //boulder 2017_04_19 18:16
//            //app.UseMvc(routes =>
//            //{


//            //    foreach(var qqqqq123 in qqqqqs123)
//            //    {
//            //        routes.MapRoute(
//            //            name: qqqqq123.Name,
//            //            template: qqqqq123.Template,
//            //            defaults: qqqqq123.Defaults
//            //            );
//            //    }






//                ////below works
//                ////routes.MapRoute(
//                ////    "Default", // Route name
//                ////    "{controller}/{action}/{id}", // URL with parameters
//                ////    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
//                ////    new[] { "Grand.Web.Controllers" }
//                ////);
//                ////this is new equivalent of above previous 
//                //routes.MapRoute(
//                //    name: "default",
//                //    template: "{controller=Home}/{action=Index}/{id?}"
//                //    );

//                ////works fine
//                //routes.MapRoute(
//                //name: "Category",
//                //template: "{SeName}",
//                //defaults: new { controller = "Catalog", action = "Category" });
//            //});

//            //routes.IgnoreRoute("favicon.ico");
//            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

//            //register custom routes (plugins, etc)

//            //ex
//            //var test1 = EngineContextExperimental.Current.Resolve<ITypeFinder>();
















//            //tests below












//            //app.UseMvc(routes =>
//            //{
//            //    //test
//            //    //    //this is new equivalent of above previous 
//            //    routes.MapRoute(
//            //        name: "default",
//            //        template: "{controller=Home}/{action=Index}/{id?}"
//            //        );

//            //});

//            //this order works fine
//            //app.UseMvc(routes =>
//            //{
//            //    routes.MapRoute(
//            //        name: "default",
//            //        template: "{controller=Home}/{action=Index}/{id?}"
//            //        );

//            //    routes.MapRoute(
//            //        name: "Category",
//            //        template: "{SeName}",
//            //        defaults: new { controller = "Catalog", action = "Category" });
//            //});

//            //works too
//            //app.UseMvc(routes =>
//            //{

//            //    routes.MapRoute(
//            //        name: "Category",
//            //        template: "{SeName}",
//            //        defaults: new { controller = "Catalog", action = "Category" });

//            //    routes.MapRoute(
//            //        name: "default",
//            //        template: "{controller=Home}/{action=Index}/{id?}");

//            //});


//            //doesnt work
//            //app.UseMvc(routes =>
//            //{
//            //    routes.MapRoute(
//            //        name: "default",
//            //        template: "{controller=Home}/{action=Index}/{id?}");
//            //});
//            //app.UseMvc(routes =>
//            //{
//            //    routes.MapRoute(
//            //        name: "Category",
//            //        template: "{SeName}",
//            //        defaults: new { controller = "Catalog", action = "Category" });
//            //});

//            //doesnt work too
//            //app.UseMvc(routes =>
//            //{
//            //    routes.MapRoute(
//            //        name: "Category",
//            //        template: "{SeName}",
//            //        defaults: new { controller = "Catalog", action = "Category" });
//            //});
//            //app.UseMvc(routes =>
//            //{
//            //    routes.MapRoute(
//            //        name: "default",
//            //        template: "{controller=Home}/{action=Index}/{id?}");
//            //});









//        //}


//    }
//}
