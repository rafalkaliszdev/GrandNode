using System;
using System.Collections.Generic;
//using System.Web.Routing;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Payments;
using Grand.Core.Plugins;
using Grand.Plugin.Payments.PayByBitcoin.Controllers;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Orders;
using Grand.Services.Payments;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Payments.PayByBitcoin
{
    /// <summary>
    /// PayByBitcoin payment processor
    /// </summary>
    public class PayByBitcoinPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        private readonly PayByBitcoinPaymentSettings _PayByBitcoinPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor

        public PayByBitcoinPaymentProcessor(PayByBitcoinPaymentSettings PayByBitcoinPaymentSettings,
            ISettingService settingService, IOrderTotalCalculationService orderTotalCalculationService,
            ILocalizationService localizationService)
        {
            this._PayByBitcoinPaymentSettings = PayByBitcoinPaymentSettings;
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._localizationService = localizationService;
        }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _PayByBitcoinPaymentSettings.AdditionalFee, _PayByBitcoinPaymentSettings.AdditionalFeePercentage);
            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayByBitcoin";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Grand.Plugin.Payments.PayByBitcoin.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayByBitcoin";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Grand.Plugin.Payments.PayByBitcoin.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayByBitcoinController);
        }

        public override void Install()
        {
            var settings = new PayByBitcoinPaymentSettings()
            {
                DescriptionText = "<p>Reserve items at your local store, and pay in store when you pick up your order.<br />Our store location: USA, New York,...</p><p>P.S. You can edit this text from admin panel.</p>"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.PaymentMethodDescription", "Pay In Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFee.Hint", "The additional fee.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");

            
            base.Install();
        }
        
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PayByBitcoinPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.PayByBitcoin.PaymentMethodDescription");

            base.Uninstall();
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        public string PaymentMethodDescription
        {
            get
            {
                return _localizationService.GetResource("Plugins.Payment.PayByBitcoin.PaymentMethodDescription");
            }
        }
        #endregion

    }
}
