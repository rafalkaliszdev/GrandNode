//using System.Web.Mvc;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Plugin.Shipping.ShipByDrone.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        //[AllowHtml]
        [GrandResourceDisplayName("Plugins.Payment.ShipByDrone.DescriptionText")]
        public string DescriptionText { get; set; }

        [GrandResourceDisplayName("Plugins.Payment.ShipByDrone.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [GrandResourceDisplayName("Plugins.Payment.ShipByDrone.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
    }
}