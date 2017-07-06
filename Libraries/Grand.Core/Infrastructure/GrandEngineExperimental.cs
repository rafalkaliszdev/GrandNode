using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Grand.Core.Configuration;
using Grand.Core.Infrastructure.DependencyManagement;
using Grand.Core.Infrastructure.Mapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grand.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// </summary>
    public class GrandEngineExperimental : IEngineExperimental
    {
        #region Fields

        private /*IContainer*/ContainerManager _containerManager;

        #endregion

        public GrandEngineExperimental(/*IContainer*/ContainerManager container)
        {
            this._containerManager = container;
        }

        #region Methods


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
            //return null;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public virtual /*IContainer*/ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion

    }
}
