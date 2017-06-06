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
    public class EngineContextExperimental
    {
        #region Methods

        /// <summary>
        /// Initializes a static instance of the Grand factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.NoInlining/*Synchronized*/)]
        public static IEngineExperimental Initialize(IContainer container)
        {
            if (Singleton<IEngineExperimental>.Instance == null)
            {
                Singleton<IEngineExperimental>.Instance = new GrandEngineExperimental(container);
            }
            return Singleton<IEngineExperimental>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngineExperimental engine)
        {
            Singleton<IEngineExperimental>.Instance = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static IEngineExperimental Current
        {
            get
            {
                if (Singleton<IEngineExperimental>.Instance == null)
                {
                    throw new InvalidOperationException("the ll is this");
                    //Initialize();
                }
                return Singleton<IEngineExperimental>.Instance;
            }
        }

        #endregion
    }
}
