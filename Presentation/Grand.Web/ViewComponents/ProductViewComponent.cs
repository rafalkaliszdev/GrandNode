using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Services.Catalog;
using Grand.Services.Customers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;
using Autofac;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Orders;
using Grand.Services.Events;
using Grand.Services.Orders;
using Grand.Web.Framework.Security.Captcha;
using Grand.Web.Infrastructure.Cache;
using System;

namespace Grand.Web.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IProductWebService _productWebService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IOrderReportService _orderReportService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerActionEventService _customerActionEventService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IOrderService _orderService;

        public ProductViewComponent(
            IProductService productService,
            IProductWebService productWebService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            //ICompareProductsService compareProductsService,
            IOrderReportService orderReportService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            ICustomerActionEventService customerActionEventService,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            //ICacheManager cacheManager,
            IOrderService orderService
            )
        {
            this._productService = productService;
            this._productWebService = productWebService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            //this._compareProductsService = compareProductsService;
            this._orderReportService = orderReportService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._customerActionEventService = customerActionEventService;
            this._eventPublisher = eventPublisher;
            this._catalogSettings = catalogSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            //this._cacheManager = cacheManager;
            this._orderService = orderService;

        }

        public async Task<IViewComponentResult> InvokeAsync(string actionName, int? productThumbPictureSize)
        {
            switch (actionName)
            {
                case nameof(this.HomepageBestSellers):
                    return await HomepageBestSellers(productThumbPictureSize);
                case nameof(this.HomepageProducts):
                    return await HomepageProducts(productThumbPictureSize);
                case nameof(this.RecommendedProducts):
                    return await RecommendedProducts(productThumbPictureSize);
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
        }

        #region Home page bestseller, recommended and products

        public virtual async Task<IViewComponentResult> HomepageBestSellers(int? productThumbPictureSize)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            //load and cache report
            var report =
                //_cacheManager.Get(string.Format(ModelCacheEventConsumer.HOMEPAGE_BESTSELLERS_IDS_KEY, _storeContext.CurrentStore.Id),
                //() => 
                _orderReportService.BestSellersReport(
                        storeId: _storeContext.CurrentStore.Id,
                        pageSize: _catalogSettings.NumberOfBestsellersOnHomepage)
                        .ToList();
                        //);


            //load products
            var products = _productService.GetProductsByIds(report.Select(x => x.ProductId).ToArray());
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = _productWebService.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return View(nameof(this.HomepageBestSellers), model);
        }

        public virtual async Task<IViewComponentResult> HomepageProducts(int? productThumbPictureSize)
        {
            var products = _productService.GetAllProductsDisplayedOnHomePage();

            //ACL and store mapping
            products = products.ToList();//.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            var model = _productWebService.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            return View(nameof(this.HomepageProducts), model);

            //return View("/Views/Shared/Components/Product/HomepageProducts.cshtml", model);
        }

        #endregion

        public virtual async Task<IViewComponentResult> RecommendedProducts(int? productThumbPictureSize)
        {
          if (!_catalogSettings.RecommendedProductsEnabled)
                return Content("");

            var products = _productService.GetRecommendedProducts(_workContext.CurrentCustomer.GetCustomerRoleIds());

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();

            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = _productWebService.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            return View(nameof(this.RecommendedProducts), model);


            //return View(model);
        }
    }
}