using System.Collections.Generic;
//using System.Web.Mvc;
using Grand.Plugin.Shipping.ShipByDrone.Models;
using Grand.Services.Configuration;
using Grand.Services.Shipping;
using Grand.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace Grand.Plugin.Shipping.ShipByDrone.Controllers
{
    public class ShippingShipByDroneController : BaseShippingController
    {
        private readonly ISettingService _settingService;
        private readonly ShipByDronePaymentSettings _ShipByDronePaymentSettings;

        public ShippingShipByDroneController(ISettingService settingService, ShipByDronePaymentSettings ShipByDronePaymentSettings)
        {
            this._settingService = settingService;
            this._ShipByDronePaymentSettings = ShipByDronePaymentSettings;
        }

        //[AdminAuthorize]
        //[ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DescriptionText = _ShipByDronePaymentSettings.DescriptionText;
            model.AdditionalFee = _ShipByDronePaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = _ShipByDronePaymentSettings.AdditionalFeePercentage;

            return View("~/Plugins/Shipping.ShipByDrone/Views/PaymentShipByDrone/Configure.cshtml", model);
        }

        [HttpPost]
        //[AdminAuthorize]
        //[ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _ShipByDronePaymentSettings.DescriptionText = model.DescriptionText;
            _ShipByDronePaymentSettings.AdditionalFee = model.AdditionalFee;
            _ShipByDronePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_ShipByDronePaymentSettings);

            return Configure();
        }

        public override JsonResult GetFormPartialView(string shippingOption)
        {
            var qq = base.RenderPartialViewToString();
            return Json(qq);
        }

        public override IList<string> ValidateShippingForm(IFormCollection form)
        {
            return new List<string>();
        }
    }
}