using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grand.Web.Services;

namespace Grand.Web.ViewComponents
{
    public class CatalogViewComponent : ViewComponent
    {
        #region Methods

        private readonly ICatalogWebService _catalogWebService;

        #endregion

        #region Constructors

        public CatalogViewComponent(
            ICatalogWebService catalogWebService
            )
        {
            this._catalogWebService = catalogWebService;
        }

        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            switch (actionName)
            {
                case nameof(this.TopMenu):
                    return await TopMenu();
                case nameof(this.HomepageCategories):
                    return await HomepageCategories();
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IViewComponentResult> TopMenu()
        {
            var model = /*await*/ _catalogWebService.PrepareTopMenu();
            return View(nameof(this.TopMenu), model);
        }

        public virtual async Task<IViewComponentResult> HomepageCategories()
        {
            var model = /*await*/ _catalogWebService.PrepareHomepageCategory();
            if (!model.Any())
                return Content("");

            return View(nameof(this.HomepageCategories), model);
        }

        #endregion

    }
}