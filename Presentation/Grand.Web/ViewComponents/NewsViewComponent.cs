using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Grand.Core;
using Grand.Core.Domain.News;
using Grand.Web.Services;

namespace Grand.Web.ViewComponents
{
    public class NewsViewComponent : ViewComponent
    {
        #region Fields

        private readonly INewsWebService _newsWebService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly NewsSettings _newsSettings;

        #endregion

        #region Constructors

        public NewsViewComponent(
            INewsWebService newsWebService,
            IWorkContext workContext, 
            IStoreContext storeContext, 
            IWebHelper webHelper, 
            NewsSettings newsSettings)
        {
            this._newsWebService = newsWebService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._newsSettings = newsSettings;
        }

        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            switch (actionName)
            {
                case nameof(this.HomePageNews):
                    return  HomePageNews();
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
        }

        #endregion

        #region Methods

        public virtual IViewComponentResult HomePageNews()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
                return Content("");

            var model = _newsWebService.PrepareHomePageNewsItems();
            return View(nameof(this.HomePageNews), model);
        }

        #endregion
    }
}