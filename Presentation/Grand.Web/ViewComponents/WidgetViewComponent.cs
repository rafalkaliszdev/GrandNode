using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Grand.Web.Services;

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

        #endregion

        #region Invoker

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
            var model =  /*await*/ _widgetWebService.PrepareRenderWidget(widgetZone, additionalData);

            if (model == null)
                return Content("");

            return View(nameof(WidgetsByZone), model);
        }

        #endregion
    }
}