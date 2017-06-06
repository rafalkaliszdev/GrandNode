using System;
using System.Collections.Generic;
using System.Linq;
///*using System.Web.Mvc;*/
using Autofac;
//using Autofac.Integration.Mvc;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Core.Infrastructure.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Grand.Web.Framework.Mvc.Routes;

using Autofac;
//using Autofac.Extensions.DependencyInjection;
//using Microsoft.Net;
using Grand.Core.Infrastructure;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Core.Configuration;
using Grand.Core.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
///*using System.Web.Mvc;*/
//using Autofac;
//using Autofac.Integration.Mvc;
//using AutoMapper;
//using Grand.Core.Configuration;
//using Grand.Core.Infrastructure.DependencyManagement;
//using Grand.Core.Infrastructure.Mapper;

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;


using Microsoft.AspNetCore.Mvc.Razor;

using Microsoft.AspNetCore.Mvc.ViewEngines;


using Microsoft.Extensions.DependencyInjection;
//using Grand.Web.Framework.Themes;

using Grand.Core.Configuration.Routes;

namespace Grand.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// </summary>
    public class GrandEngine //: IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="config">Config</param>
        public virtual void RegisterDependencies(GrandConfig config, IServiceCollection services, out IContainer applicationContainer, /*out RoutePublisher routePublisher, */out AutofacServiceProvider autofacServiceProvider)
        {



            //again new
            var typeFinder = new WebAppTypeFinder();
            var builder = new ContainerBuilder();

            builder.RegisterInstance(config).As<GrandConfig>().SingleInstance();
            builder.RegisterInstance(/*Singleton<IEngine>.Instance*/this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();



            


            //it needs to be here
            //builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
            //routePublisher = new RoutePublisher(typeFinder);
            //builder.RegisterInstance(routePublisher).As<IRoutePublisher>().SingleInstance();








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
             applicationContainer = builder.Build();


            //one extra line
            this._containerManager = new ContainerManager(applicationContainer);




            //here should be EngineContextExperimental
            //return new AutofacServiceProvider(applicationContainer);
            //hope it'll get thing done
            autofacServiceProvider = new AutofacServiceProvider(applicationContainer);


































            ////code below should be inside GrandEngine.RegisterDependencies() but i need autofac object here
            ////so here begins registration of services from service business layer 
            ////dependencies
            //var typeFinder = new WebAppTypeFinder();
            //var builder = new ContainerBuilder();
            ////let's register some simple Service and use it in Controller
            ////builder.RegisterType<Randomizer>().As<IRandomizer>().InstancePerLifetimeScope();

            //builder.RegisterInstance(config).As<GrandConfig>().SingleInstance();
            //builder.RegisterInstance(Singleton<IEngine>.Instance/*this*/).As<IEngine>().SingleInstance();
            //builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();


            ////it needs to be here
            ////builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
            //routePublisher = new RoutePublisher(typeFinder);
            //builder.RegisterInstance(routePublisher).As<IRoutePublisher>().SingleInstance();


            ////so get now something

            ////register dependencies provided by other assemblies
            //var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            //var drInstances = new List<IDependencyRegistrar>();
            //foreach (var drType in drTypes)
            //    drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

            ////sort
            //drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            //foreach (var dependencyRegistrar in drInstances)
            //    dependencyRegistrar.Register(builder, typeFinder, config);

            ////var container = builder.Build();
            ////this._containerManager = new ContainerManager(container);


            //builder.Populate(services);
            //applicationContainer = builder.Build();
            //this._containerManager = new ContainerManager(applicationContainer);





            //since this Method has no references, I imply that it is being called by framework
            //here should be EngineContextExperimental
























            //  var builder = new ContainerBuilder();
            //  //we create new instance of ContainerBuilder
            //  //because Build() or Update() method can only be called once on a ContainerBuilder.


            //  //dependencies
            //  var typeFinder = new WebAppTypeFinder();
            //  builder = new ContainerBuilder();
            //  builder.RegisterInstance(config).As<GrandConfig>().SingleInstance();
            //  builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            //  builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            //  //register dependencies provided by other assemblies
            //var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            //  var drInstances = new List<IDependencyRegistrar>();
            //  foreach (Type drType in drTypes)
            //      drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

            //  //sort
            //  drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            //  foreach (var dependencyRegistrar in drInstances)
            //      dependencyRegistrar.Register(builder, typeFinder, config);

            //  var container = builder.Build();
            //  this._containerManager = new ContainerManager(container);

            //  //set dependency resolver
            //  DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        /// <summary>
        /// Register mapping
        /// </summary>
        /// <param name="config">Config</param>
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

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(GrandConfig config, IServiceCollection services, out IContainer applicationContainer, /*out RoutePublisher routePublisher,*/ out AutofacServiceProvider autofacServiceProvider)
        {
            //register dependencies
            //let's comment it
            //RegisterDependencies(config, services, out applicationContainer, /*out routePublisher, */out autofacServiceProvider);

            //woa
            //applicationContainer = null;
            //routePublisher = null;
            applicationContainer = null;
            autofacServiceProvider = null;



            //register mapper configurations
            RegisterMapperConfiguration(config);
            //startup tasks
            if (!config.IgnoreStartupTasks)
            {
                //RunStartupTasks();
            }

        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public virtual ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
