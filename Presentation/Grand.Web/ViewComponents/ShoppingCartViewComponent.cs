using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Web;
/*using System.Web.Mvc;*/
using Grand.Core;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Discounts;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Shipping;
using Grand.Core.Domain.Tax;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Discounts;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Media;
using Grand.Services.Messages;
using Grand.Services.Orders;
using Grand.Services.Security;
using Grand.Services.Seo;
using Grand.Services.Tax;
using Grand.Web.Framework.Controllers;
using Grand.Web.Framework.Mvc;
using Grand.Web.Framework.Security;
using Grand.Web.Framework.Security.Captcha;
using Grand.Web.Models.ShoppingCart;
using Grand.Web.Services;
using Grand.Core.Infrastructure;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Grand.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressWebService _addressWebService;
        private readonly IShoppingCartWebService _shoppingCartWebService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Constructors

        public ShoppingCartViewComponent(
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPermissionService permissionService,
            IShoppingCartWebService shoppingCartWebService,
            IHttpContextAccessor httpContextAccessor,

            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings
            )
        {
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._permissionService = permissionService;

            this._shoppingCartWebService = shoppingCartWebService;

            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            if (actionName == nameof(this.FlyoutShoppingCart))
                return await FlyoutShoppingCart();

            return Content("sometimes you run so fast, you lose your aim out of sight");
        }

        #region Shopping cart


        ////[ChildActionOnly]
        //public virtual IActionResult OrderSummary(bool? prepareAndDisplayOrderReviewData)
        //{
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = new ShoppingCartModel();
        //    _shoppingCartWebService.PrepareShoppingCart(model, cart,
        //        isEditable: false,
        //        prepareEstimateShippingIfEnabled: false,
        //        prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
        //    return PartialView(model);
        //}


        ////[ChildActionOnly]
        //public virtual IActionResult OrderTotals(bool isEditable)
        //{
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = _shoppingCartWebService.PrepareOrderTotals(cart, isEditable);
        //    return PartialView(model);
        //}

        //[ChildActionOnly]
        public virtual async Task<IViewComponentResult> FlyoutShoppingCart()
        {
            var _shoppingCartSettings = new ShoppingCartSettings()
            {
                MiniShoppingCartEnabled = true,
            };

            if (!_shoppingCartSettings.MiniShoppingCartEnabled)
                return Content("");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return Content("");

            var model = _shoppingCartWebService.PrepareMiniShoppingCart();

            return View("/Views/Shared/Components/ShoppingCart/FlyoutShoppingCart.cshtml", model);
        }

        #endregion

        //#region Wishlist





        //#endregion
    }
}
