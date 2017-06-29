using System;
using System.Collections.Generic;
//using System.Web.Routing;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Shipping;
using Grand.Core.Plugins;
using Grand.Plugin.Shipping.ShipByDrone.Controllers;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Orders;
using Grand.Services.Shipping;
using Grand.Services.Shipping.Tracking;
using Microsoft.AspNetCore.Routing;

namespace Grand.Plugin.Shipping.ShipByDrone
{
    /// <summary>
    /// ShipByDrone payment processor
    /// </summary>
    public class ShipByDronePaymentComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields
        private readonly ShipByDronePaymentSettings _ShipByDronePaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor

        public ShipByDronePaymentComputationMethod(ShipByDronePaymentSettings ShipByDronePaymentSettings,
            ISettingService settingService, IOrderTotalCalculationService orderTotalCalculationService,
            ILocalizationService localizationService)
        {
            this._ShipByDronePaymentSettings = ShipByDronePaymentSettings;
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentShipByDrone";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Grand.Plugin.Shipping.ShipByDrone.Controllers" }, { "area", null } };
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
            controllerName = "PaymentShipByDrone";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Grand.Plugin.Shipping.ShipByDrone.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(Controllers.ShippingShipByDroneController);
        }

        public override void Install()
        {
            var settings = new ShipByDronePaymentSettings()
            {
                DescriptionText = "<p>Reserve items at your local store, and pay in store when you pick up your order.<br />Our store location: USA, New York,...</p><p>P.S. You can edit this text from admin panel.</p>"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.PaymentMethodDescription", "Pay In Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFee.Hint", "The additional fee.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");


            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ShipByDronePaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.ShipByDrone.PaymentMethodDescription");

            base.Uninstall();
        }

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            var response = new GetShippingOptionResponse
            {
                ShippingOptions =
                {
                    new ShippingOption
                    {
                        Name = "ship by drone",
                        Description = "descr",
                        Rate = 2494,
                        ShippingRateComputationMethodSystemName ="Shipping.ShipByDrone"
                    }
                }
            };
            return response;
        }

        public bool HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return (decimal)2494;
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
                return _localizationService.GetResource("Plugins.Payment.ShipByDrone.PaymentMethodDescription");
            }
        }

        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Offline;

        public IShipmentTracker ShipmentTracker
        {
            get { return null; }
        }
        #endregion

    }
}
