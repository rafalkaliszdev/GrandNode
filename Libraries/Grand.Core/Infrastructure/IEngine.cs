﻿using System;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure.DependencyManagement;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Grand.Core.Configuration.Routes;
using Autofac.Extensions.DependencyInjection;

namespace Grand.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the Nop engine. Edit functionality, modules
    /// and implementations access most Nop functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Container manager
        /// </summary>
        /*ContainerManager*/IContainer ContainerManager { get; }

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="config">Config</param>
        //void Initialize(GrandConfig config, IServiceCollection services, out IContainer applicationContainer, /*out RoutePublisher routePublisher,*/ out AutofacServiceProvider autofacServiceProvider);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}
