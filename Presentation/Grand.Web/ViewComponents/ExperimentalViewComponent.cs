using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Grand.Web.Services;

namespace Grand.Web.ViewComponents
{
    public class ExperimentalViewComponent : ViewComponent
    {
        #region Fields

        private readonly IWidgetWebService _widgetWebService;

        #endregion

        #region Constructors

        public ExperimentalViewComponent(IWidgetWebService widgetWebService)
        {
            this._widgetWebService = widgetWebService;
        }

        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string actionName, string widgetZone, object additionalData = null)
        {
            return View("Fqfnqfwqnfw");
        }

        #endregion        
    }
}