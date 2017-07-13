﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Web;
using Microsoft.AspNetCore.Mvc;
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

namespace Grand.Web.Controllers
{
    public partial class ShoppingCartController : BasePublicController
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

        public ShoppingCartController(
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IDiscountService discountService,
            ICustomerService customerService,
            IGiftCardService giftCardService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressWebService addressWebService,
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
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._discountService = discountService;
            this._customerService = customerService;
            this._giftCardService = giftCardService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._addressWebService = addressWebService;
            this._httpContextAccessor = httpContextAccessor;
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

        #region Shopping cart

        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        //public virtual IActionResult AddProductToCart_Catalog()
        public virtual IActionResult AddProductToCart_Catalog(string productId, int shoppingCartTypeId, int quantity, bool forceredirection = false)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;
            var product = _productService.GetProductById(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.CustomerEntersPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.IsRental)
            {
                //rental products require start/end dates to be entered
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            var allowedQuantities = product.ParseAllowedQuantities();
            if (allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            if (product.ProductAttributeMappings.Any())
            {
                //product has some attributes. let a customer see them
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == cartType)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();









            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product.Id);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;


            //hope it wont be needed
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, cartType,
                product, _storeContext.CurrentStore.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //let's display standard warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = _shoppingCartService.AddToCart(customer: _workContext.CurrentCustomer,
                productId: productId,
                shoppingCartType: cartType,
                storeId: _storeContext.CurrentStore.Id,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                        _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts());
                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = /*"here should be info about current quantity "; //*/string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                        _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts());

                        var updateflyoutcartsectionhtml = /*false;*/ _shoppingCartSettings.MiniShoppingCartEnabled
                            ? this.RenderPartialViewToString("FlyoutShoppingCart", _shoppingCartWebService.PrepareMiniShoppingCart())
                            : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                        });
                    }
            }
        }

        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        //[ValidateInput(false)]
        public virtual IActionResult AddProductToCart_Details(string productId, int shoppingCartTypeId, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple products could be added to the cart"
                });
            }

            #region Update existing shopping cart item?
            string updatecartitemid = "";
            foreach (string formKey in form.Keys)
                if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.OrdinalIgnoreCase))
                {
                    updatecartitemid = form[formKey];
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && !String.IsNullOrEmpty(updatecartitemid))
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartTypeId == shoppingCartTypeId)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);

                //is it this product?
                if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This product does not match a passed shopping cart item identifier"
                    });
                }
            }
            #endregion

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (string formKey in form.Keys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.OrdinalIgnoreCase))
                    {
                        decimal customerEnteredPrice;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;
            foreach (string formKey in form.Keys)
                if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion

            //product and gift card attributes
            string attributes = _shoppingCartWebService.ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                _shoppingCartWebService.ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                        //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                        updatecartitem.ShoppingCartType;

            //save item
            var addToCartWarnings = new List<string>();
            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                    productId, cartType, _storeContext.CurrentStore.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == updatecartitem.ShoppingCartType)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                    cart, updatecartitem.ShoppingCartType, productId, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's other shopping cart cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
                if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                {
                    //delete the same shopping cart item (the other one)
                    _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                }
            }

            #region Return result

            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                        _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts());

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                        _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList()
                        .GetTotalProducts());

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? this.RenderPartialViewToString("FlyoutShoppingCart", _shoppingCartWebService.PrepareMiniShoppingCart())
                            : "";

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
                        });
                    }
            }


            #endregion
        }

        //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the product details pages
        [HttpPost]
        //[ValidateInput(false)]
        public virtual IActionResult ProductDetails_AttributeChange(string productId, bool validateAttributeConditions, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return new NullJsonResult(null);

            string attributeXml = _shoppingCartWebService.ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                _shoppingCartWebService.ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            string sku = product.FormatSku(attributeXml, _productAttributeParser);
            string mpn = product.FormatMpn(attributeXml, _productAttributeParser);
            string gtin = product.FormatGtin(attributeXml, _productAttributeParser);


            string price = "";
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices) && !product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                List<Discount> scDiscounts;
                decimal discountAmount;
                decimal finalPrice = _priceCalculationService.GetUnitPrice(product,
                    _workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out discountAmount, out scDiscounts);
                decimal taxRate;
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out taxRate);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
            }
            //stock
            var stockAvailability = product.FormatStockMessage(attributeXml, _localizationService, _productAttributeParser, _storeContext);

            //conditional attributes
            var enabledAttributeMappingIds = new List<string>();
            var disabledAttributeMappingIds = new List<string>();
            if (validateAttributeConditions)
            {
                var attributes = product.ProductAttributeMappings;
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(product, attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            return Json(new
            {
                gtin = gtin,
                mpn = mpn,
                sku = sku,
                price = price,
                stockAvailability = stockAvailability,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray()
            });
        }

        [HttpPost]
        //[ValidateInput(false)]
        public virtual IActionResult CheckoutAttributeChange(IFormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            _shoppingCartWebService.ParseAndSaveCheckoutAttributes(cart, form);
            var attributeXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes,
                _storeContext.CurrentStore.Id);

            var enabledAttributeIds = new List<string>();
            var disabledAttributeIds = new List<string>();
            var attributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in attributes)
            {
                var conditionMet = _checkoutAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            return Json(new
            {
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        public virtual IActionResult UploadFileProductAttribute(string attributeId, string productId)
        {
            var product = _productService.GetProductById(productId);
            var attribute = product.ProductAttributeMappings.Where(x => x.Id == attributeId).FirstOrDefault();
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                });//, "text/plain");
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            //if (String.IsNullOrEmpty(Request["qqfile"]))
            //{
            //    // IE
            //    HttpPostedFileBase httpPostedFile = Request.Files[0];
            //    if (httpPostedFile == null)
            //        throw new ArgumentException("No file uploaded");
            //    stream = httpPostedFile.InputStream;
            //    fileName = Path.GetFileName(httpPostedFile.FileName);
            //    contentType = httpPostedFile.ContentType;
            //}
            //else
            //{
            //    //Webkit, Mozilla
            //    stream = Request.InputStream;
            //    fileName = Request["qqfile"];
            //}

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty,
                    });//, "text/plain");
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            });//, "text/plain");
        }

        [HttpPost]
        public virtual IActionResult UploadFileCheckoutAttribute(string attributeId)
        {
            var attribute = _checkoutAttributeService.GetCheckoutAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                });//, "text/plain");
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            //if (String.IsNullOrEmpty(Request["qqfile"]))
            //{
            //    // IE
            //    HttpPostedFileBase httpPostedFile = Request.Files[0];
            //    if (httpPostedFile == null)
            //        throw new ArgumentException("No file uploaded");
            //    stream = httpPostedFile.InputStream;
            //    fileName = Path.GetFileName(httpPostedFile.FileName);
            //    contentType = httpPostedFile.ContentType;
            //}
            //else
            //{
            //    //Webkit, Mozilla
            //    stream = Request.InputStream;
            //    fileName = Request["qqfile"];
            //}

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty,
                    });//, "text/plain");
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            });//, "text/plain");
        }


        //////[GrandHttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Cart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                //.LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartModel();
            _shoppingCartWebService.PrepareShoppingCart(model, cart);
            return View(model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public virtual IActionResult UpdateCart(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList() : new List<string>();

            ////current warnings <cart item identifier, warnings>
            //var innerWarnings = new Dictionary<string, IList<string>>();
            //foreach (var sci in cart)
            //{
            //    bool remove = allIdsToRemove.Contains(sci.Id);
            //    if (remove)
            //        _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
            //    else
            //    {
            //        foreach (string formKey in form.Keys)
            //            if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.OrdinalIgnoreCase))
            //            {
            //                int newQuantity;
            //                if (int.TryParse(form[formKey], out newQuantity))
            //                {
            //                    var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
            //                        sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
            //                        sci.RentalStartDateUtc, sci.RentalEndDateUtc,
            //                        newQuantity, true);
            //                    innerWarnings.Add(sci.Id, currSciWarnings);
            //                }
            //                break;
            //            }
            //    }
            //}
            ////parse and save checkout attributes
            //_shoppingCartWebService.ParseAndSaveCheckoutAttributes(cart, form);

            ////updated cart
            //_workContext.CurrentCustomer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);
            //cart = _workContext.CurrentCustomer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            var model = new ShoppingCartModel();
            //_shoppingCartWebService.PrepareShoppingCart(model, cart);
            ////update current warnings
            //foreach (var kvp in innerWarnings)
            //{
            //    //kvp = <cart item identifier, warnings>
            //    var sciId = kvp.Key;
            //    var warnings = kvp.Value;
            //    //find model
            //    var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
            //    if (sciModel != null)
            //        foreach (var w in warnings)
            //            if (!sciModel.Warnings.Contains(w))
            //                sciModel.Warnings.Add(w);
            //}
            return View(model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("continueshopping")]
        public virtual IActionResult ContinueShopping()
        {
            var returnUrl = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LastContinueShoppingPage, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToRoute("HomePage");
            }
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public virtual IActionResult StartCheckout(IFormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            _shoppingCartWebService.ParseAndSaveCheckoutAttributes(cart, form);

            //validate attributes
            var checkoutAttributes = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _storeContext.CurrentStore.Id);
            var checkoutAttributeWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributes, true);
            if (checkoutAttributeWarnings.Any())
            {
                //something wrong, redisplay the page with warnings
                var model = new ShoppingCartModel();
                _shoppingCartWebService.PrepareShoppingCart(model, cart, validateCheckoutAttributes: true);
                return View(model);
            }

            //everything is OK
            if (_workContext.CurrentCustomer.IsGuest())
            {
                if (!_orderSettings.AnonymousCheckoutAllowed)
                    return new UnauthorizedResult();

                return RedirectToRoute("LoginCheckoutAsGuest", new { returnUrl = Url.RouteUrl("ShoppingCart") });
            }

            return RedirectToRoute("Checkout");
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applydiscountcouponcode")]
        public virtual IActionResult ApplyDiscountCoupon(string discountcouponcode, IFormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            _shoppingCartWebService.ParseAndSaveCheckoutAttributes(cart, form);

            var model = new ShoppingCartModel();
            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetDiscountByCouponCode(discountcouponcode, true);
                if (discount != null && discount.RequiresCouponCode)
                {
                    var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, discountcouponcode);
                    if (validationResult.IsValid)
                    {
                        //valid
                        _workContext.CurrentCustomer.ApplyDiscountCouponCode(discountcouponcode);
                        model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(validationResult.UserError))
                        {
                            //some user error
                            model.DiscountBox.Message = validationResult.UserError;
                            model.DiscountBox.IsApplied = false;
                        }
                        else
                        {
                            //general error text
                            model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                            model.DiscountBox.IsApplied = false;
                        }
                    }
                }
                else
                {
                    model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                    model.DiscountBox.IsApplied = false;
                }
            }
            else
            {
                model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                model.DiscountBox.IsApplied = false;
            }

            _shoppingCartWebService.PrepareShoppingCart(model, cart);
            return View(model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("applygiftcardcouponcode")]
        public virtual IActionResult ApplyGiftCard(string giftcardcouponcode, IFormCollection form)
        {
            //trim
            if (giftcardcouponcode != null)
                giftcardcouponcode = giftcardcouponcode.Trim();

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            _shoppingCartWebService.ParseAndSaveCheckoutAttributes(cart, form);

            var model = new ShoppingCartModel();
            if (!cart.IsRecurring())
            {
                if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
                    if (isGiftCardValid)
                    {
                        _workContext.CurrentCustomer.ApplyGiftCardCouponCode(giftcardcouponcode);
                        //_customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                        model.GiftCardBox.IsApplied = true;
                    }
                    else
                    {
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                        model.GiftCardBox.IsApplied = false;
                    }
                }
                else
                {
                    model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                    model.GiftCardBox.IsApplied = false;
                }
            }
            else
            {
                model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");
                model.GiftCardBox.IsApplied = false;
            }

            _shoppingCartWebService.PrepareShoppingCart(model, cart);
            return View(model);
        }

        //[ValidateInput(false)]
        ////[PublicAntiForgery]
        [HttpPost]
        public virtual IActionResult GetEstimateShipping(string countryId, string stateProvinceId, string zipPostalCode, IFormCollection form)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //parse and save checkout attributes
            _shoppingCartWebService.ParseAndSaveCheckoutAttributes(cart, form);
            var model = _shoppingCartWebService.PrepareEstimateShippingResult(cart, countryId, stateProvinceId, zipPostalCode);

            return PartialView("_EstimateShippingResult", model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removediscount-")]
        public virtual IActionResult RemoveDiscountCoupon(IFormCollection form)
        {
            var model = new ShoppingCartModel();
            string discountId = string.Empty;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removediscount-", StringComparison.OrdinalIgnoreCase))
                    discountId = formValue.Substring("removediscount-".Length);

            var discount = _discountService.GetDiscountById(discountId);
            if (discount != null)
            {
                _workContext.CurrentCustomer.RemoveDiscountCouponCode(discount.CouponCode);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            _shoppingCartWebService.PrepareShoppingCart(model, cart);
            return View(model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Cart")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removegiftcard-")]
        public virtual IActionResult RemoveGiftCardCode(IFormCollection form)
        {
            var model = new ShoppingCartModel();

            //get gift card identifier
            string giftCardId = "";
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("removegiftcard-", StringComparison.OrdinalIgnoreCase))
                    giftCardId = formValue.Substring("removegiftcard-".Length);
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
            {
                _workContext.CurrentCustomer.RemoveGiftCardCouponCode(gc.GiftCardCouponCode);
                //_customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            _shoppingCartWebService.PrepareShoppingCart(model, cart);
            return View(model);
        }

        #endregion

        #region Wishlist

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Wishlist(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            Customer customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToRoute("HomePage");
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistModel();
            _shoppingCartWebService.PrepareWishlist(model, cart, !customerGuid.HasValue);
            return View(model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("updatecart")]
        public virtual IActionResult UpdateWishlist(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //var allIdsToRemove = form["removefromcart"] != null
            //    ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            //    .Select(x => x)
            //    .ToList()
            //    : new List<string>();

            ////current warnings <cart item identifier, warnings>
            //var innerWarnings = new Dictionary<string, IList<string>>();
            //foreach (var sci in cart)
            //{
            //    bool remove = allIdsToRemove.Contains(sci.Id);
            //    if (remove)
            //        _shoppingCartService.DeleteShoppingCartItem(sci);
            //    else
            //    {
            //        foreach (string formKey in form.Keys)
            //            if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.OrdinalIgnoreCase))
            //            {
            //                int newQuantity;
            //                if (int.TryParse(form[formKey], out newQuantity))
            //                {
            //                    var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
            //                        sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
            //                        sci.RentalStartDateUtc, sci.RentalEndDateUtc,
            //                        newQuantity, true);
            //                    innerWarnings.Add(sci.Id, currSciWarnings);
            //                }
            //                break;
            //            }
            //    }
            //}

            ////updated wishlist
            //_workContext.CurrentCustomer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);

            //cart = _workContext.CurrentCustomer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            var model = new WishlistModel();
            //_shoppingCartWebService.PrepareWishlist(model, cart);
            ////update current warnings
            //foreach (var kvp in innerWarnings)
            //{
            //    //kvp = <cart item identifier, warnings>
            //    var sciId = kvp.Key;
            //    var warnings = kvp.Value;
            //    //find model
            //    var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
            //    if (sciModel != null)
            //        foreach (var w in warnings)
            //            if (!sciModel.Warnings.Contains(w))
            //                sciModel.Warnings.Add(w);
            //}
            return View(model);
        }

        //[ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired("addtocartbutton")]
        public virtual IActionResult AddItemsToCartFromWishlist(Guid? customerGuid, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("HomePage");

            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return RedirectToRoute("HomePage");

            var pageCart = pageCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;
            //var allIdsToAdd = form["addtocart"] != null
            //    ? form["addtocart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            //    .Select(x => x)
            //    .ToList()
            //    : new List<string>();
            //foreach (var sci in pageCart)
            //{
            //    if (allIdsToAdd.Contains(sci.Id))
            //    {
            //        var product = _productService.GetProductById(sci.ProductId);
            //        var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
            //            sci.ProductId, ShoppingCartType.ShoppingCart,
            //            _storeContext.CurrentStore.Id,
            //            sci.AttributesXml, sci.CustomerEnteredPrice,
            //            sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
            //        if (!warnings.Any())
            //            numberOfAddedItems++;
            //        if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
            //            !customerGuid.HasValue && //own wishlist
            //            !warnings.Any()) //no warnings ( already in the cart)
            //        {
            //            //let's remove the item from wishlist
            //            _shoppingCartService.DeleteShoppingCartItem(sci);
            //        }
            //        allWarnings.AddRange(warnings);
            //    }
            //}

            if (numberOfAddedItems > 0)
            {
                //redirect to the shopping cart page

                if (allWarnings.Any())
                {
                    ErrorNotification(_localizationService.GetResource("Wishlist.AddToCart.Error"), true);
                }

                return RedirectToRoute("ShoppingCart");
            }
            else
            {
                //no items added. redisplay the wishlist page

                if (allWarnings.Any())
                {
                    ErrorNotification(_localizationService.GetResource("Wishlist.AddToCart.Error"), false);
                }
                //no items added. redisplay the wishlist page
                var cart = pageCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var model = new WishlistModel();
                _shoppingCartWebService.PrepareWishlist(model, cart, !customerGuid.HasValue);
                return View(model);
            }
        }

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult EmailWishlist()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (!cart.Any())
                return RedirectToRoute("HomePage");

            var model = new WishlistEmailAFriendModel
            {
                YourEmailAddress = _workContext.CurrentCustomer.Email,
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage
            };
            return View(model);
        }

        [HttpPost, ActionName("EmailWishlist")]
        [FormValueRequired("send-email")]
        ////[PublicAntiForgery]
        ////[CaptchaValidator]
        public virtual IActionResult EmailWishlistSend(WishlistEmailAFriendModel model, bool captchaValid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) || !_shoppingCartSettings.EmailWishlistEnabled)
                return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            //check whether the current customer is guest and ia allowed to email wishlist
            if (_workContext.CurrentCustomer.IsGuest() && !_shoppingCartSettings.AllowAnonymousUsersToEmailWishlist)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Wishlist.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                EngineContextExperimental.Current.Resolve<IWorkflowMessageService>().SendWishlistEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, model.YourEmailAddress,
                        model.FriendEmail, Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Wishlist.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage;
            return View(model);
        }

        #endregion
    }
}
