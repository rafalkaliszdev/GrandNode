//using System.Web.Mvc;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Plugin.Payments.PayByBitcoin.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        //[AllowHtml]
        [GrandResourceDisplayName("Plugins.Payment.PayByBitcoin.DescriptionText")]
        public string DescriptionText { get; set; }

        [GrandResourceDisplayName("Plugins.Payment.PayByBitcoin.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [GrandResourceDisplayName("Plugins.Payment.PayByBitcoin.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
    }
}