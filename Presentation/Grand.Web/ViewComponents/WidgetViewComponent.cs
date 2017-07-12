using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grand.Web.Services;
using Microsoft.AspNetCore.Routing;

namespace Grand.Web.ViewComponents
{
    public class WidgetViewComponent : ViewComponent
    {
        #region Fields

        private readonly IWidgetWebService _widgetWebService;

        #endregion

        #region Constructors

        public WidgetViewComponent(IWidgetWebService widgetWebService)
        {
            this._widgetWebService = widgetWebService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string actionName, string widgetZone, object additionalData = null)
        {
            switch (actionName)
            {
                case nameof(this.WidgetsByZone):
                    return await WidgetsByZone(widgetZone, additionalData);
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IViewComponentResult> WidgetsByZone(string widgetZone, object additionalData = null)
        {
            //var model =  /*await*/ _widgetWebService.PrepareRenderWidget(widgetZone, additionalData);            
            //temporary workaround
            var model = new List<Models.Cms.RenderWidgetModel>
            {
                new  Models.Cms.RenderWidgetModel
                {
                    ActionName = "HomepageCategories",
                    ControllerName = "Catalog",//this should be actually 'ViewComponentName'
                    RouteValues = new RouteValueDictionary(new { qq = "dqwdqw"})
                }
            };

            if (model == null)
                return Content("");

            return View(nameof(WidgetsByZone), model);
        }

        #endregion
    }
}