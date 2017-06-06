using System.Configuration;
using System.Runtime.CompilerServices;
using Grand.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Grand.Core.Configuration.Routes;
using System;
using Autofac.Extensions.DependencyInjection;

namespace Grand.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Nop engine.
    /// </summary>
    public class EngineContext
    {
        #region Methods

        /// <summary>
        /// Initializes a static instance of the Grand factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.NoInlining/*Synchronized*/)]
        public static /*IEngine*/Tuple<IContainer,/* RoutePublisher, */AutofacServiceProvider> Initialize(bool forceRecreate, IServiceCollection services, /*out*/ IContainer applicationContainer, /*out *//*RoutePublisher routePublisher,*/ /*out */AutofacServiceProvider autofacServiceProvider)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                //Singleton<IEngine>.Instance = new GrandEngine();

                //var config = ConfigurationManager.GetSection("GrandConfig") as GrandConfig;
                var config = new GrandConfig();
                //Singleton<IEngine>.Instance.Initialize(config, services, out applicationContainer,/* out routePublisher,*/ out autofacServiceProvider);

                //so stupid, but can work
                applicationContainer = applicationContainer;
                //routePublisher = routePublisher;
            }

            //woa
            //applicationContainer = null;
            //routePublisher = null;

            return new Tuple<IContainer, /*RoutePublisher,*/ AutofacServiceProvider>(applicationContainer, /*routePublisher,*/autofacServiceProvider);


            //return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    //i cant do it now
                    //Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
