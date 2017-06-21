using Autofac;
using Autofac.Extensions.DependencyInjection;
using Grand.Core.Configuration;
using Grand.Core.Configuration.Routes;
using Grand.Core.Infrastructure;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Web.Framework.Themes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Grand.Core.Infrastructure.Mapper;
using AutoMapper;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Infrastructure;
//using Grand.Core.Infrastructure.Extensions;
//using Grand.Web.Framework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Grand.Core.Plugins;
using System.Reflection;

namespace Grand.Web
{
    public class StartupDevelopment : IEngine
    {
        public static IContainer _container { get; private set; } //cns in gn_origin it is ContainerManger + maybe static or readonly ?


        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;






        public StartupDevelopment(
            IHostingEnvironment hostingEnvironment,
            ILoggerFactory loggerFactory
            )
        {
            this._hostingEnvironment = hostingEnvironment;
            this._loggerFactory = loggerFactory;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {


            //just some testing, didnt want to make new vs project

            var qwq = Assembly.GetEntryAssembly();

            

            //asp.net core mvc creates an instance of this class
            //so there is no need to make singleton



            //LoggerFactory service
            _loggerFactory.AddDebug();




            //Themeable Razor View Engine
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
            });





            //init plugins
            var mvcCoreBuilder = services.AddMvc();
            //services.AddMvcCore();         
            PluginManager.Initialize(mvcCoreBuilder.PartManager);


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();


            //ctl make it from appSettings.json
            var config = new GrandConfig();

            //register autofac dependencies
            var autofacServiceProvider = this.RegisterDependencies(services, config);

            //register mapper configurations
            this.RegisterMapperConfiguration(config);

            //startup tasks
            if (false)//!config.IgnoreStartupTasks)
                this.RunStartupTasks();











            //new
            //return new AutofacServiceProvider(applicationContainer);
            //return autofacServiceProvider;
            //previous
            return autofacServiceProvider;
        }



        public AutofacServiceProvider RegisterDependencies(IServiceCollection services, GrandConfig config)
        {

            //2017_05_18
            //not cut, just copied into GrandEngine and commented

            //code below should be inside GrandEngine.RegisterDependencies() but i need autofac object here


            var typeFinder = new WebAppTypeFinder();
            var builder = new ContainerBuilder();

            builder.RegisterInstance(config).As<GrandConfig>().SingleInstance();

            //tbh, I hope it is possible
            builder.RegisterInstance(this/*Singleton<IEngine>.Instance*/).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();



            //need to get rid of this from here - now it will be in DependencyRegistrar
            //_routePublisher = new RoutePublisher(typeFinder);
            //builder.RegisterInstance(_routePublisher).As<IRoutePublisher>().SingleInstance();






            //register dependencies provided by other assemblies
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder, config);

            builder.Populate(services);
            /*this.*/
            /*var*/ _container = builder.Build();

            //so it is done here, because i really only need this EngineContextExperimental and GrandEngine for IContainer
            EngineContextExperimental.Initialize(_container);


            //var test01 = EngineContextExperimental.Current.ContainerManager.Resolve<IThemeProvider>();


            //here should be EngineContextExperimental
            return new AutofacServiceProvider(_container);











            //previous way from gn_origin
            ////set dependency resolver
            ////DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected virtual void RegisterMapperConfiguration(GrandConfig config)
        {
            //dependencies
            var typeFinder = new WebAppTypeFinder();

            //register mapper configurations provided by other assemblies
            var mcTypes = typeFinder.FindClassesOfType<IMapperConfiguration>();
            var mcInstances = new List<IMapperConfiguration>();
            foreach (var mcType in mcTypes)
                mcInstances.Add((IMapperConfiguration)Activator.CreateInstance(mcType));
            //sort
            mcInstances = mcInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            //get configurations
            var configurationActions = new List<Action<IMapperConfigurationExpression>>();
            foreach (var mc in mcInstances)
                configurationActions.Add(mc.GetConfiguration());
            //register
            AutoMapperConfiguration.Init(configurationActions);
        }

        protected virtual void RunStartupTasks()
        {
            //var typeFinder = _containerManager.Resolve<ITypeFinder>();

            //tbh, can easily resolve from not-wrapped IContainer ?


            //previous
            var typeFinder = _container.Resolve<ITypeFinder>();
            //new
            //var typeFinder = EngineContextExperimental.Current.Resolve<ITypeFinder>();


            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }






















        public void ConfigureDevelopment(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "NopCookie",
                CookieHttpOnly = true
            });

            loggerFactory.AddConsole();



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //Each Use extension method adds a middleware component to the request pipeline. 







            //rtl unnecessary part of this (
            //static files
            app.UseStaticFiles();// For the wwwroot folder
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "scripts")),
                RequestPath = new PathString("/scripts")
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "scripts")),
                RequestPath = new PathString("/scripts")
            });















            //RegisterRoutes(app);

            //use private filed
            //_routePublisher.RegisterRoutes(app);


            //use IContainer
            //previous
            _container.Resolve<IRoutePublisher>().RegisterRoutes(app);
            //new
            //EngineContextExperimental.Current.Resolve<IRoutePublisher>().RegisterRoutes(app);








            app.Run(async (context) =>
            {
                if (context.Request.Path == "/favicon.ico")
                {
                    await context.Response.SendFileAsync("favicon.ico");
                }
                else
                {
                    //await context.Response.SendFileAsync("qqqqq.txt");
                    //await context.Response.WriteAsync("Hello World!");
                }

            });


        }


        //so it wont be called if there is ConfigureDevelopment
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        //{
        //    loggerFactory.AddConsole();



        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //        app.UseBrowserLink();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //    }
        //    //Each Use extension method adds a middleware component to the request pipeline. F
        //    app.UseStaticFiles();





        //    //RegisterRoutes(app);
        //    _routePublisher.RegisterRoutes(app);


        //    app.Run(async (context) =>
        //    {
        //        if (context.Request.Path == "/favicon.ico")
        //        {
        //            await context.Response.SendFileAsync("favicon.ico");
        //        }
        //        else
        //        {
        //            //await context.Response.SendFileAsync("favicon.ico");

        //            await context.Response.SendFileAsync("qqqqq.txt");
        //            await context.Response.WriteAsync("Hello World!");
        //        }

        //    });
        //}















        public /*ContainerManager */IContainer ContainerManager => throw new NotImplementedException();

        public void Initialize(GrandConfig config, IServiceCollection services, out IContainer applicationContainer, out AutofacServiceProvider autofacServiceProvider)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            throw new NotImplementedException();
        }

        public static RoutePublisher _routePublisher;


    }
}
