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
using Grand.Core.Infrastructure;
using Grand.Web.Framework.Themes;
using Grand.Web.Framework.UI;

//using static Autofac.ResolutionExtensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;


namespace Grand.Web.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly ICatalogWebService _catalogWebService;
        private readonly IProductService _productService;
        private readonly IProductWebService _productWebService;

        public ProductViewComponent(
            ICatalogWebService catalogWebService,
            IProductService productService,
            IProductWebService productWebService

            )
        {
            this._catalogWebService = catalogWebService;
            this._productService = productService;
            this._productWebService = productWebService;

        }

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            if (actionName == nameof(this.HomepageProducts))
                return await HomepageProducts();


            return Content("sometimes you run so fast, you lose your aim out of sight");
        }

        public virtual async Task<IViewComponentResult>/*IActionResult*/ HomepageProducts()//(int? productThumbPictureSize)
        {
            //var model = _catalogWebService.PrepareHomepageCategory();
            //if (!model.Any())
            //    return Content("");

            //return View("HomepageCategories", model);



           
            var products = _productService.GetAllProductsDisplayedOnHomePage();

            //ACL and store mapping
            products = products.ToList();//.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");


            var model = _productWebService.PrepareProductOverviewModels(products, true, true, 300/*productThumbPictureSize*/).ToList();
            //return /*Partial*/View("HomepageProducts.cshtml", model);//Views/Shared/Components/Product/



            //yeah explicit path works
            return /*Partial*/View("/Views/Shared/Components/Product/HomepageProducts.cshtml", model);//Views/Shared/Components/Product/
        }
    }
}
