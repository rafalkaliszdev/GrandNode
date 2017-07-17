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
using Grand.Web.Models.Catalog;
using System.Collections.Generic;

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
                case nameof(this.SuggestedProducts):
                    return await SuggestedProducts(productThumbPictureSize);

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
        
        public virtual async Task<IViewComponentResult> SuggestedProducts(int? productThumbPictureSize)
        {
            if (!_catalogSettings.SuggestedProductsEnabled || _catalogSettings.SuggestedProductsNumber == 0)
                return Content("");

            var products = _productService.GetSuggestedProducts(_workContext.CurrentCustomer.CustomerTags.ToArray());

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();

            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = _productWebService.PrepareProductOverviewModels(products.Take(_catalogSettings.SuggestedProductsNumber), true, true, productThumbPictureSize).ToList();
            return View(nameof(this.SuggestedProducts), model);

            return View(model);
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


            //throw new NotImplementedException("add correct return view");
        }

        #region Product details page

        //[ChildActionOnly]
        public virtual async Task<IViewComponentResult> RelatedProducts(string productId, int? productThumbPictureSize)
        {
            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () =>
                    _productService.GetProductById(productId).RelatedProducts.Select(x => x.ProductId2).ToArray()
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            var model = _productWebService.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            throw new NotImplementedException("add correct return view");
        }

        //[ChildActionOnly]
        public virtual async Task<IViewComponentResult> ProductsAlsoPurchased(string productId, int? productThumbPictureSize)
        {
            if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
                return Content("");

            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_ALSO_PURCHASED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () =>
                    _orderReportService
                    .GetAlsoPurchasedProductsIds(_storeContext.CurrentStore.Id, productId, _catalogSettings.ProductsAlsoPurchasedNumber)
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = _productWebService.PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            throw new NotImplementedException("add correct return view");
        }

        //[ChildActionOnly]
        public virtual async Task<IViewComponentResult> CrossSellProducts(int? productThumbPictureSize)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");


            //Cross-sell products are dispalyed on the shopping cart page.
            //We know that the entire shopping cart page is not refresh
            //even if "ShoppingCartSettings.DisplayCartAfterAddingProduct" setting  is enabled.
            //That's why we force page refresh (redirect) in this case
            var model = _productWebService.PrepareProductOverviewModels(products,
                productThumbPictureSize: productThumbPictureSize, forceRedirectionAfterAddingToCart: true)
                .ToList();

            throw new NotImplementedException("add correct return view");
        }

        #endregion

        #region Recently viewed products

        //[ChildActionOnly]
        public virtual async Task<IViewComponentResult> RecentlyViewedProductsBlock(int? productThumbPictureSize, bool? preparePriceModel)
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
                return Content("");

            var preparePictureModel = productThumbPictureSize.HasValue;
            var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_workContext.CurrentCustomer.Id, _catalogSettings.RecentlyViewedProductsNumber);

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (!products.Any())
                return Content("");

            //2017_07_14 18:52
            //boulder
            //you can start here

            //prepare model
            var model = new List<ProductOverviewModel>();
            model.AddRange(_productWebService.PrepareProductOverviewModels(products,
                preparePriceModel.GetValueOrDefault(),
                preparePictureModel,
                productThumbPictureSize));

            throw new NotImplementedException("add correct return view");
        }

        #endregion
    }
}