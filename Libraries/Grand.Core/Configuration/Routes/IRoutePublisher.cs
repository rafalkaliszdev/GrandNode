/*using System.Web.Routing;*/
using Microsoft.AspNetCore.Builder;

//namespace Grand.Web.Framework.Mvc.Routes
namespace Grand.Core.Configuration.Routes
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public interface IRoutePublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        void RegisterRoutes(IApplicationBuilder app);
    }
}
