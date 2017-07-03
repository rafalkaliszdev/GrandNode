using System.Collections.Generic;
//using System.Web.Mvc;
using Grand.Plugin.Payments.PayByBitcoin.Models;
using Grand.Services.Configuration;
using Grand.Services.Payments;
using Grand.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Grand.Core.Infrastructure;

namespace Grand.Plugin.Payments.PayByBitcoin.Controllers
{
    public class PaymentPayByBitcoinController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly PayByBitcoinPaymentSettings _PayByBitcoinPaymentSettings;

        //rtl need this because i need barameterless constructor
        public PaymentPayByBitcoinController()
        {
            this._settingService = EngineContextExperimental.Current.Resolve<ISettingService>(); ;
            this._PayByBitcoinPaymentSettings = EngineContextExperimental.Current.Resolve<PayByBitcoinPaymentSettings>();
        }

        public PaymentPayByBitcoinController(ISettingService settingService, PayByBitcoinPaymentSettings PayByBitcoinPaymentSettings)
        {
            this._settingService = settingService;
            this._PayByBitcoinPaymentSettings = PayByBitcoinPaymentSettings;
        }


        //[AdminAuthorize]
        //[ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DescriptionText = _PayByBitcoinPaymentSettings.DescriptionText;
            model.AdditionalFee = _PayByBitcoinPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = _PayByBitcoinPaymentSettings.AdditionalFeePercentage;

            return View("~/Plugins/Payments.PayByBitcoin/Views/PaymentPayByBitcoin/Configure.cshtml", model);
        }

        [HttpPost]
        //[AdminAuthorize]
        //[ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _PayByBitcoinPaymentSettings.DescriptionText = model.DescriptionText;
            _PayByBitcoinPaymentSettings.AdditionalFee = model.AdditionalFee;
            _PayByBitcoinPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_PayByBitcoinPaymentSettings);

            return Configure();
        }

        //[ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel()
            {
                DescriptionText = _PayByBitcoinPaymentSettings.DescriptionText
            };

            return View("~/Plugins/Payments.PayByBitcoin/Views/PaymentPayByBitcoin/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(IFormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }
    }
}