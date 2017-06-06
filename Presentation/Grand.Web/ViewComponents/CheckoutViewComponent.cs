using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Domain.Shipping;
using Grand.Core.Plugins;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Orders;
using Grand.Services.Payments;
using Grand.Services.Shipping;
using Grand.Services.Stores;
using Grand.Services.Tax;
using Grand.Web.Models.Checkout;
using Grand.Web.Models.Common;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Web.ViewComponents
{
    public class CheckoutViewComponent : ViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAddressWebService _addressWebService;
        private readonly IRewardPointsService _rewardPointsService;

        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Constructors
        public CheckoutViewComponent(
                        IWorkContext workContext,
                        IStoreContext storeContext,
                        IStoreMappingService storeMappingService,
                        IShoppingCartService shoppingCartService,
                        ILocalizationService localizationService,
                        ITaxService taxService,
                        ICurrencyService currencyService,
                        IPriceFormatter priceFormatter,
                        IOrderProcessingService orderProcessingService,
                        ICustomerService customerService,
                        IGenericAttributeService genericAttributeService,
                        ICountryService countryService,
                        IStateProvinceService stateProvinceService,
                        IShippingService shippingService,
                        IPaymentService paymentService,
                        IPluginFinder pluginFinder,
                        IOrderTotalCalculationService orderTotalCalculationService,
                        ILogger logger,
                        IOrderService orderService,
                        IWebHelper webHelper,
                        IHttpContextAccessor httpContextAccessor,
                        IAddressWebService addressWebService,
                        IRewardPointsService rewardPointsService,
                        OrderSettings orderSettings,
                        RewardPointsSettings rewardPointsSettings,
                        PaymentSettings paymentSettings,
                        ShippingSettings shippingSettings
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._pluginFinder = pluginFinder;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._logger = logger;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._httpContextAccessor = httpContextAccessor;
            this._addressWebService = addressWebService;
            this._rewardPointsService = rewardPointsService;
            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;

        }

        #endregion

        #region Invoker 

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            if (actionName == nameof(this.OpcBillingForm))
                return await OpcBillingForm();

            return Content("non existing action in scope of this view component");
        }

        #endregion

        #region actions

        public virtual async Task<IViewComponentResult> OpcBillingForm()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var billingAddressModel = PrepareBillingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);
            return View("OpcBillingAddress", billingAddressModel);
        }

        #endregion


        #region Utilities

        [NonAction]
        protected virtual CheckoutBillingAddressModel PrepareBillingAddressModel(
        IList<ShoppingCartItem> cart, string selectedCountryId = null,
        bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressModel();
            model.ShipToSameAddressAllowed = _shippingSettings.ShipToSameAddress && cart.RequiresShipping();
            model.ShipToSameAddress = true;

            //existing addresses

            var addresses = _workContext.CurrentCustomer.Addresses
                .Where(a => a.CountryId == "" ||
                (_countryService.GetCountryById(a.CountryId) != null ? _countryService.GetCountryById(a.CountryId).AllowsBilling : false)
                )
                .Where(a => a.CountryId == "" ||
                _storeMappingService.Authorize((_countryService.GetCountryById(a.CountryId))
                ))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                _addressWebService.PrepareModel(model: addressModel, address: address, excludeProperties: false);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            _addressWebService.PrepareModel(model: model.NewAddress, address: null, excludeProperties: false,
                loadCountries: () => _countryService.GetAllCountriesForBilling(_workContext.WorkingLanguage.Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer,
                overrideAttributesXml: overrideAttributesXml
                );
            return model;
        }

        #endregion
    }
}
