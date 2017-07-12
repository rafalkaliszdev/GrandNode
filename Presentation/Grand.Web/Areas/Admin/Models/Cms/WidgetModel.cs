using Microsoft.AspNetCore.Mvc.Rendering;
/*using System.Web.Routing;*/
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Grand.Web.Areas.Admin.Models.Cms
{
    public partial class WidgetModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.ContentManagement.Widgets.Fields.FriendlyName")]
        /*[AllowHtml]*/
        public string FriendlyName { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Widgets.Fields.SystemName")]
        /*[AllowHtml]*/
        public string SystemName { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Widgets.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Widgets.Fields.IsActive")]
        public bool IsActive { get; set; }
        

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}