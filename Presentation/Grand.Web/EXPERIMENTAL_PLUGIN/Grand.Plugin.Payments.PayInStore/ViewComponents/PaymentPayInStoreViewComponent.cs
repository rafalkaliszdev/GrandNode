//using System;
//using System.Collections.Generic;
//using System.Web.Mvc;
//using Grand.Plugin.Payments.PayInStore.Models;
//using Grand.Services.Configuration;
//using Grand.Services.Payments;
//using Grand.Web.Framework.Controllers;
//using Microsoft.AspNetCore.Mvc;

//namespace Grand.Web.EXPERIMENTAL_PLUGIN.Grand.Plugin.Payments.PayInStore.ViewComponents
//{
//    class PaymentPayInStoreViewComponent : BasePaymentViewComponent
//    {
//        private readonly ISettingService _settingService;
//        private readonly PayInStorePaymentSettings _payInStorePaymentSettings;

//        public PaymentPayInStoreViewComponent(ISettingService settingService, PayInStorePaymentSettings payInStorePaymentSettings)
//        {
//            this._settingService = settingService;
//            this._payInStorePaymentSettings = payInStorePaymentSettings;
//        }

//        //[AdminAuthorize]
//        //[ChildActionOnly]
//        public ActionResult Configure()
//        {
//            var model = new ConfigurationModel();
//            model.DescriptionText = _payInStorePaymentSettings.DescriptionText;
//            model.AdditionalFee = _payInStorePaymentSettings.AdditionalFee;
//            model.AdditionalFeePercentage = _payInStorePaymentSettings.AdditionalFeePercentage;

//            return View("~/Plugins/Payments.PayInStore/Views/PaymentPayInStore/Configure.cshtml", model);
//        }

//        [HttpPost]
//        //[AdminAuthorize]
//        //[ChildActionOnly]
//        public ActionResult Configure(ConfigurationModel model)
//        {
//            if (!ModelState.IsValid)
//                return Configure();

//            //save settings
//            _payInStorePaymentSettings.DescriptionText = model.DescriptionText;
//            _payInStorePaymentSettings.AdditionalFee = model.AdditionalFee;
//            _payInStorePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
//            _settingService.SaveSetting(_payInStorePaymentSettings);

//            return Configure();
//        }

//        //[ChildActionOnly]
//        public ActionResult PaymentInfo()
//        {
//            var model = new PaymentInfoModel()
//            {
//                DescriptionText = _payInStorePaymentSettings.DescriptionText
//            };

//            return View("~/Plugins/Payments.PayInStore/Views/PaymentPayInStore/PaymentInfo.cshtml", model);
//        }

//        [NonAction]
//        public override IList<string> ValidatePaymentForm(IFormCollection form)
//        {
//            var warnings = new List<string>();
//            return warnings;
//        }

//        [NonAction]
//        public override ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
//        {
//            var paymentInfo = new ProcessPaymentRequest();
//            return paymentInfo;
//        }
//    }
//}