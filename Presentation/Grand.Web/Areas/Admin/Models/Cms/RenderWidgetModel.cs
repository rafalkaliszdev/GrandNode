﻿/*using System.Web.Routing;*/
using Grand.Web.Framework.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Grand.Web.Areas.Admin.Models.Cms
{
    public partial class RenderWidgetModel : BaseNopModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}