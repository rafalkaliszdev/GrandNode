using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
/*using System.Web.Mvc;*/
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Vendors;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Services.Vendors;
using Grand.Web.Framework.Security;
using Grand.Web.Models.Catalog;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.ViewComponents
{
    public class CatalogViewComponent : ViewComponent
    {
        private readonly ICatalogWebService _catalogWebService;

        public CatalogViewComponent(
            ICatalogWebService catalogWebService
            )
        {
            this._catalogWebService = catalogWebService;
        }

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

        public virtual async Task<IViewComponentResult> TopMenu()
        {
            var model = /*await*/ _catalogWebService.PrepareTopMenu();
            return View("TopMenu", model);
        }

        public virtual async Task<IViewComponentResult> HomepageCategories()
        {
            var model = /*await*/ _catalogWebService.PrepareHomepageCategory();
            if (!model.Any())
                return Content("");

            return View("HomepageCategories", model);
        }
    }
}