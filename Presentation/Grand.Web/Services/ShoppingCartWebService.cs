﻿using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Directory;
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
using Grand.Services.Media;
using Grand.Services.Orders;
using Grand.Services.Payments;
using Grand.Services.Security;
using Grand.Services.Seo;
using Grand.Services.Shipping;
using Grand.Services.Tax;
using Grand.Web.Infrastructure.Cache;
using Grand.Web.Models.Common;
using Grand.Web.Models.Media;
using Grand.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
/*using System.Web;*/
///*using System.Web.Mvc;*/
///*using System.Web.Routing;*/

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


//woa
using Grand.Web.EXPERIMENTAL_PLUGIN.Grand.Plugin.Payments.PayInStore;
using Grand.Services.Configuration;

namespace Grand.Web.Services
{
    public partial class ShoppingCartWebService : IShoppingCartWebService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICurrencyService _currencyService;
        private readonly IDiscountService _discountService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly ITaxService _taxService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressWebService _addressWebService;
        private readonly IShippingService _shippingService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IGenericAttributeService _genericAttributeService;

        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;


        public ShoppingCartWebService(
            IHttpContextAccessor httpContextAccessor,
            //ICacheManager cacheManager,
            IWorkContext workContext,
            IWebHelper webHelper,
            IPaymentService paymentService,
            IProductService productService,
            //IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            ILocalizationService localizationService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IOrderProcessingService orderProcessingService,
            ICurrencyService currencyService,
            IDiscountService discountService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ICheckoutAttributeService checkoutAttributeService,
            IPermissionService permissionService,
            ITaxService taxService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            IDownloadService downloadService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressWebService addressWebService,
            IShippingService shippingService,
            IProductAttributeFormatter productAttributeFormatter,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IGenericAttributeService genericAttributeService,

            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            //this._cacheManager = cacheManager;
            this._workContext = workContext;
            this._webHelper = webHelper;
            this._paymentService = paymentService;
            this._productService = productService;
            //this._pictureService = pictureService;
            this._productAttributeParser = productAttributeParser;
            this._localizationService = localizationService;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._currencyService = currencyService;
            this._discountService = discountService;
            this._shoppingCartService = shoppingCartService;
            this._storeContext = storeContext;
            this._checkoutAttributeService = checkoutAttributeService;
            this._permissionService = permissionService;
            this._taxService = taxService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._downloadService = downloadService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressWebService = addressWebService;
            this._shippingService = shippingService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceCalculationService = priceCalculationService;
            this._genericAttributeService = genericAttributeService;

            this._mediaSettings = mediaSettings;
            this._orderSettings = orderSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
        }

        public virtual PictureModel PrepareCartItemPicture(ShoppingCartItem sci,
            int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, sci.ProductId, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(pictureCacheKey,
                //as we cache per user (shopping cart item identifier)
                //let's cache just for 3 minutes
                3, () =>
                {
                    var sciPicture = _productService.GetProductById(sci.ProductId).GetProductPicture(sci.AttributesXml, _pictureService, _productAttributeParser);
                    return new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(sciPicture, _mediaSettings.ApplyWatermarkForProduct, pictureSize, showDefaultPicture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                    };
                });

            return model;
        }

        public virtual void PrepareShoppingCart(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (!cart.Any())
                return;

            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            //var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _storeContext.CurrentStore.Id);
            //model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
            bool minOrderSubtotalAmountOk = true;// _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                //decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                //model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //gift card and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            //var discountCouponCodes = _workContext.CurrentCustomer.ParseAppliedDiscountCouponCodes();
            //foreach (var couponCode in discountCouponCodes)
            //{
            //    var discount = _discountService.GetDiscountByCouponCode(couponCode);
            //    if (discount != null &&
            //        discount.RequiresCouponCode &&
            //        _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
            //    {
            //        model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel()
            //        {
            //            Id = discount.Id,
            //            CouponCode = discount.CouponCode
            //        });
            //    }
            //}

            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            //var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            //foreach (var warning in cartWarnings)
            //    model.Warnings.Add(warning);

            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = attribute.CheckoutAttributeValues;
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attributeValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                var selectedCheckoutAttributes = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _storeContext.CurrentStore.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var attributeValue in selectedValues)
                                    if (attributeModel.Id == attributeValue.CheckoutAttributeId)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Any())
                            {
                                DateTime selectedDate;
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }

                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var downloadGuidStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                                Guid downloadGuid;
                                Guid.TryParse(downloadGuidStr, out downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                    attributeModel.DefaultValue = download.DownloadGuid.ToString();
                            }
                        }
                        break;
                    default:
                        break;
                }

                model.CheckoutAttributes.Add(attributeModel);
            }

            #endregion 

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                model.EstimateShipping.Enabled = cart.Any() && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
                if (model.EstimateShipping.Enabled)
                {
                    //countries
                    string defaultEstimateCountryId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "" });
                    foreach (var c in _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id))
                        model.EstimateShipping.AvailableCountries.Add(new SelectListItem
                        {
                            Text = c.GetLocalized(x => x.Name),
                            Value = c.Id.ToString(),
                            Selected = c.Id == defaultEstimateCountryId
                        });
                    //states
                    string defaultEstimateStateId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
                    var states = !String.IsNullOrEmpty(defaultEstimateCountryId) ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId, _workContext.WorkingLanguage.Id).ToList() : new List<StateProvince>();
                    if (states.Any())
                        foreach (var s in states)
                            model.EstimateShipping.AvailableStates.Add(new SelectListItem
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = s.Id == defaultEstimateStateId
                            });
                    else
                        model.EstimateShipping.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "" });

                    if (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null)
                        model.EstimateShipping.ZipPostalCode = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode;
                }
            }

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var product = _productService.GetProductById(sci.ProductId);
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = product.Sku,// product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = product.Id,
                    ProductName = product.GetLocalized(x => x.Name),
                    ProductSeName = product.GetSeName(),
                    Quantity = sci.Quantity,
                    //AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                    product.ProductType == ProductType.SimpleProduct &&
                    (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || product.IsGiftCard) &&
                    product.VisibleIndividually;

                //disable removal?
                //1. do other items require this one?
                if (product.RequireOtherProducts)
                    cartItemModel.DisableRemoval = product.RequireOtherProducts && product.ParseRequiredProductIds().Intersect(cart.Select(x => x.ProductId)).Any();

                //allowed quantities
                var allowedQuantities = product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), product.RecurringCycleLength, product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    List<Discount> scDiscounts;
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    //cartItemModel.Picture = PrepareCartItemPicture(sci,
                    //    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Button payment methods




            //woa
            //var paymentMethods = _paymentService
            //    .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
            //    .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
            //    .Where(pm => !pm.HidePaymentMethod(cart))
            //    .ToList();


            var paymentSettings = new PayInStorePaymentSettings
            {
                AdditionalFee = 666,
                AdditionalFeePercentage = false,
                DescriptionText = "pej yn stor pejment sytins"
            };


            var paymentMethods = new List<IPaymentMethod>
            {
                new PayInStorePaymentProcessor(
                    paymentSettings,
                    Grand.Core.Infrastructure.EngineContextExperimental.Current.Resolve<ISettingService>(),
                    Grand.Core.Infrastructure.EngineContextExperimental.Current.Resolve<IOrderTotalCalculationService>(),
                    Grand.Core.Infrastructure.EngineContextExperimental.Current.Resolve<ILocalizationService>()                    
                    ),
            };


            //2017_05_29 15:15
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;


                //tbh sincerely ll it
                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                pm.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);

                model.ButtonPaymentMethodActionNames.Add(actionName);
                model.ButtonPaymentMethodControllerNames.Add(controllerName);
                model.ButtonPaymentMethodRouteValues.Add(routeValues);
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                    _addressWebService.PrepareModel(model: model.OrderReviewData.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false);

                //shipping info
                if (cart.RequiresShipping())
                {
                    model.OrderReviewData.IsShippable = true;

                    var pickupPoint = _workContext.CurrentCustomer
                       .GetAttribute<string>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);

                    model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && !String.IsNullOrEmpty(pickupPoint);

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                            _addressWebService.PrepareModel(model: model.OrderReviewData.ShippingAddress,
                                address: shippingAddress,
                                excludeProperties: false);
                    }
                    else
                    {
                        var pickup = _shippingService.GetPickupPointById(pickupPoint);
                        if (pickup != null)
                        {
                            var country = _countryService.GetCountryById(pickup.Address.CountryId);
                            model.OrderReviewData.PickupAddress = new AddressModel
                            {
                                Address1 = pickup.Address.Address1,
                                City = pickup.Address.City,
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = pickup.Address.ZipPostalCode
                            };
                        }
                    }
                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                    {
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                        model.OrderReviewData.ShippingAdditionDescription = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.ShippingOptionAttributeDescription, _storeContext.CurrentStore.Id);
                    }
                }
                //payment info
                var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";

                //custom values

                //tbh, no idea what this shit is
                ProcessPaymentRequest a = new ProcessPaymentRequest();// await _httpContextAccessor.HttpContext.Session.LoadAsync();// .Session["OrderPaymentInfo"] as ProcessPaymentRequest;


                //2017_05_29
                //oh ll it will be harsh
                //_httpContextAccessor.HttpContext.Session.Get("OrderPaymentInfo")
                //_httpContextAccessor.HttpContext.Session.("OrderPaymentInfo")


                //var processPaymentRequest = _httpContextAccessor.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                var processPaymentRequest = a;
                if (processPaymentRequest != null)
                {
                    model.OrderReviewData.CustomValues = processPaymentRequest.CustomValues;
                }
            }
            #endregion
        }

        public virtual void PrepareWishlist(WishlistModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            if (!cart.Any())
                return;

            #region Simple properties

            var customer = cart.GetCustomer();
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = customer.GetFullName();
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnWishList;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var product = _productService.GetProductById(sci.ProductId);
                var cartItemModel = new WishlistModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = product.Id,
                    ProductName = product.GetLocalized(x => x.Name),
                    ProductSeName = product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing && product.ProductType == ProductType.SimpleProduct && (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || product.IsGiftCard) && product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }


                //recurring info
                if (product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), product.RecurringCycleLength, product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    List<Discount> scDiscounts;
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnWishList)
                {
                    cartItemModel.Picture = PrepareCartItemPicture(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion
        }

        public virtual MiniShoppingCartModel PrepareMiniShoppingCart()
        {
            var model = new MiniShoppingCartModel
            {
                ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
                CurrentCustomerIsGuest = _workContext.CurrentCustomer.IsGuest(),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };
                

            //performance optimization (use "HasShoppingCartItems" property)
            if (_workContext.CurrentCustomer.HasShoppingCartItems)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    //.LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                model.TotalProducts = cart.GetTotalProducts();
                if (cart.Any())
                {

                    //just see if it works

                    //subtotal
                    decimal orderSubTotalDiscountAmountBase;
                    List<Discount> orderSubTotalAppliedDiscounts;
                    decimal subTotalWithoutDiscountBase;
                    decimal subTotalWithDiscountBase;
                    var subTotalIncludingTax =  _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                    _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                        out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                        out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                    decimal subtotalBase = subTotalWithoutDiscountBase;
                    decimal subtotal =  _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                    model.SubTotal = _priceFormatter.FormatPrice(subtotal, false, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                    var requiresShipping = cart.RequiresShipping();
                    //a customer should visit the shopping cart page (hide checkout button) before going to checkout if:
                    //1. "terms of service" are enabled
                    //2. min order sub-total is OK
                    //3. we have at least one checkout attribute
                    //var checkoutAttributesExistCacheKey = string.Format(ModelCacheEventConsumer.CHECKOUTATTRIBUTES_EXIST_KEY,
                    //    _storeContext.CurrentStore.Id, requiresShipping);
                    //var checkoutAttributesExist = _cacheManager.Get(checkoutAttributesExistCacheKey,
                    //    () =>
                    //    {
                    //        var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !requiresShipping);
                    //        return checkoutAttributes.Any();
                    //    });

                    //bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
                    model.DisplayCheckoutButton = true;// !_orderSettings.TermsOfServiceOnShoppingCartPage &&
                        //minOrderSubtotalAmountOk &&
                        //!checkoutAttributesExist;

                    //products. sort descending (recently added products)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(_shoppingCartSettings.MiniShoppingCartProductNumber)
                        .ToList())
                    {
                        var product = _productService.GetProductById(sci.ProductId);
                        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = product.Id,
                            ProductName = product.GetLocalized(x => x.Name),
                            ProductSeName = product.GetSeName(),
                            Quantity = sci.Quantity,
                            AttributeInfo = _productAttributeFormatter.FormatAttributes(product, sci.AttributesXml)
                        };

                        //unit prices
                        if (product.CallForPrice)
                        {
                            cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                        }
                        else
                        {
                            decimal taxRate;
                            decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                            decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                        }

                        //picture
                        if (_shoppingCartSettings.ShowProductImagesInMiniShoppingCart)
                        {
                            //cartItemModel.Picture = PrepareCartItemPicture(sci,
                            //    _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }

            return model;
        }

        public virtual OrderTotalsModel PrepareOrderTotals(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;

            if (cart.Any())
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                List<Discount> orderSubTotalAppliedDiscounts;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                }


                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates;
                    decimal shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out taxRates);
                    decimal shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                decimal orderTotalDiscountAmountBase;
                List<Discount> orderTotalAppliedDiscounts;
                List<AppliedGiftCard> appliedGiftCards;
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscounts,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Any())
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = appliedGiftCard.GiftCard.GetGiftCardRemainingAmount() - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled &&
                    _rewardPointsSettings.DisplayHowMuchWillBeEarned &&
                    shoppingCartTotalBase.HasValue)
                {
                    decimal? shippingBaseInclTax = model.RequiresShipping
                        ? _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true)
                        : 0;
                    var earnRewardPoints = shoppingCartTotalBase.Value - shippingBaseInclTax.Value;
                    if (earnRewardPoints > 0)
                        model.WillEarnRewardPoints = _orderTotalCalculationService
                            .CalculateRewardPoints(_workContext.CurrentCustomer, earnRewardPoints);
                }

            }

            return model;
        }

        public virtual void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, IFormCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                string controlId = string.Format("checkout_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, ctrlAttributes);

                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                //tbh
                                foreach (var item in cblAttributes.FirstOrDefault().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, item);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = attribute.CheckoutAttributeValues; //_checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                //tbh
                                string enteredText = ctrlAttributes.FirstOrDefault().Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _checkoutAttributeParser.RemoveCheckoutAttribute(attributesXml, attribute);
            }
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        public virtual string ParseProductAttributes(Product product, IFormCollection form)
        {
            string attributesXml = "";

            #region Product attributes
            var productAttributes = product.ProductAttributeMappings; //_productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, ctrlAttributes);
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                //tbh
                                foreach (var item in ctrlAttributes.FirstOrDefault().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!String.IsNullOrEmpty(item))
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, item);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = attribute.ProductAttributeValues; //_productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                //tbh
                                string enteredText = ctrlAttributes.FirstOrDefault().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(product, attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";

                //tbh
                foreach (string formKey in form.Keys)//.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.OrdinalIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }

        public virtual void ParseRentalDates(Product product, IFormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", product.Id);
            string endControlId = string.Format("rental_end_date_{0}", product.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }
        public virtual EstimateShippingResultModel PrepareEstimateShippingResult(List<ShoppingCartItem> cart, string countryId, string stateProvinceId, string zipPostalCode)
        {
            var model = new EstimateShippingResultModel();

            if (cart.RequiresShipping())
            {
                var address = new Address
                {
                    CountryId = countryId,
                    StateProvinceId = stateProvinceId,
                    ZipPostalCode = zipPostalCode,
                };
                GetShippingOptionResponse getShippingOptionResponse = _shippingService
                    .GetShippingOptions(cart, address, "", _storeContext.CurrentStore.Id);
                if (!getShippingOptionResponse.Success)
                {
                    foreach (var error in getShippingOptionResponse.Errors)
                        model.Warnings.Add(error);
                }
                else
                {
                    if (getShippingOptionResponse.ShippingOptions.Any())
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            var soModel = new EstimateShippingResultModel.ShippingOptionModel
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,

                            };
                            //calculate discounted and taxed rate
                            List<Discount> appliedDiscounts = null;
                            decimal shippingTotal = _orderTotalCalculationService.AdjustShippingRate(shippingOption.Rate,
                                cart, out appliedDiscounts);

                            decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                            decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                            soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                            model.ShippingOptions.Add(soModel);
                        }

                        //pickup in store?
                        if (_shippingSettings.AllowPickUpInStore)
                        {
                            var pickupPoints = _shippingService.GetAllPickupPoints();
                            if (pickupPoints.Count > 0)
                            {
                                var soModel = new EstimateShippingResultModel.ShippingOptionModel
                                {
                                    Name = _localizationService.GetResource("Checkout.PickUpInStore"),
                                    Description = _localizationService.GetResource("Checkout.PickUpInStore.Description"),
                                };

                                decimal shippingTotal = pickupPoints.Max(x => x.PickupFee);
                                decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                                soModel.Price = _priceFormatter.FormatShippingPrice(rate, true);
                                model.ShippingOptions.Add(soModel);
                            }
                        }
                    }
                    else
                    {
                        model.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                    }
                }
            }
            return model;
        }
    }
}