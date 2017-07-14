using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grand.Web.Services;
using Grand.Core;
using Grand.Core.Domain.Catalog;
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

namespace Grand.Web.ViewComponents
{
    public class CatalogViewComponent : ViewComponent
    {
        #region Fields

        private readonly ICatalogWebService _catalogWebService;
        private readonly ICategoryService _categoryService;
        private readonly IProductWebService _productWebService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IProductTagService _productTagService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerActionEventService _customerActionEventService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Constructors

        public CatalogViewComponent(
            ICatalogWebService catalogWebService,
            ICategoryService categoryService,
            IProductWebService productWebService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IProductTagService productTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            ICustomerActionEventService customerActionEventService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings
            //ICacheManager cacheManager
            )
        {
            this._catalogWebService = catalogWebService;
            this._categoryService = categoryService;
            this._productWebService = productWebService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._productTagService = productTagService;
            this._genericAttributeService = genericAttributeService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._customerActionEventService = customerActionEventService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string actionName, string currentCategoryId, string currentProductId, string currentManufacturerId)
        {
            switch (actionName)
            {
                case nameof(this.CategoryNavigation):
                    return await CategoryNavigation(currentCategoryId, currentProductId);
                case nameof(this.TopMenu):
                    return await TopMenu();
                case nameof(this.HomepageCategories):
                    return await HomepageCategories();
                case nameof(this.HomepageManufacturers):
                    return HomepageManufacturers();
                case nameof(this.ManufacturerNavigation):
                    return ManufacturerNavigation(currentManufacturerId);
                case nameof(this.VendorNavigation):
                    return VendorNavigation();
                case nameof(this.PopularProductTags):
                    return PopularProductTags();
                case nameof(this.SearchBox):
                    return await SearchBox();
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
        }

        #endregion

        #region Categories

        public virtual async Task<IViewComponentResult> CategoryNavigation(string currentCategoryId, string currentProductId)
        {
            var model = _catalogWebService.PrepareCategoryNavigation(currentCategoryId, currentProductId);
            throw new NotImplementedException("add correct view");
        }

        public virtual async Task<IViewComponentResult> TopMenu()
        {
            var model = _catalogWebService.PrepareTopMenu();
            return View(nameof(this.TopMenu), model);
        }

        public virtual async Task<IViewComponentResult> HomepageCategories()
        {
            var model = _catalogWebService.PrepareHomepageCategory();
            if (!model.Any())
                return Content("");

            return View(nameof(this.HomepageCategories), model);
        }

        #endregion

        #region Manufacturers

        public virtual /*Task<*/IViewComponentResult HomepageManufacturers()
        {
            var model = _catalogWebService.PrepareHomepageManufacturers();
            if (!model.Any())
                return Content("");
            return View(nameof(this.HomepageManufacturers), model);
        }

        public virtual /*Task<*/IViewComponentResult ManufacturerNavigation(string currentManufacturerId)
        {
            if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogWebService.PrepareManufacturerNavigation(currentManufacturerId);
            if (!model.Manufacturers.Any())
                return Content("");

            throw new NotImplementedException("add correct view");
        }

        #endregion

        #region Vendors

        public virtual /*Task<*/IViewComponentResult VendorNavigation()
        {
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogWebService.PrepareVendorNavigation();
            if (!model.Vendors.Any())
                return Content("");

            throw new NotImplementedException("add correct view");
        }

        #endregion

        #region Product tags
        
        public virtual /*Task<*/IViewComponentResult PopularProductTags()
        {
            var model = _catalogWebService.PreparePopularProductTags();
            if (!model.Tags.Any())
                return Content("");

            throw new NotImplementedException("add correct view");
        }

        #endregion

        #region Searching
        
        public virtual Task<IViewComponentResult> SearchBox()
        {
            var model = _catalogWebService.PrepareSearchBox();
            throw new NotImplementedException("add correct view");
        }

        #endregion

    }
}