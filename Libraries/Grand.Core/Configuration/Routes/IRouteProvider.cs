using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
/*using System.Web.Routing;*/

//namespace Grand.Web.Framework.Mvc.Routes
namespace Grand.Core.Configuration.Routes
{
    public interface IRouteProvider
    {
        List<NameTemplateDefaults> CollectRoutes/*RegisterRoutes*/(List<NameTemplateDefaults> routes
            /*RouteCollection routes*//*IApplicationBuilder app*/);

        int Priority { get; }
    }
}
