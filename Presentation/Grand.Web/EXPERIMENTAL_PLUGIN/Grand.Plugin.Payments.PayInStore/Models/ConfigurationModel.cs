//using System.Web.Mvc;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.EXPERIMENTAL_PLUGIN.Grand.Plugin.Payments.PayInStore.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        //[AllowHtml]
        [GrandResourceDisplayName("Plugins.Payment.PayInStore.DescriptionText")]
        public string DescriptionText { get; set; }

        [GrandResourceDisplayName("Plugins.Payment.PayInStore.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [GrandResourceDisplayName("Plugins.Payment.PayInStore.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
    }
}