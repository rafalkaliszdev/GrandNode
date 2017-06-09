using Grand.Web.Framework.Mvc;

namespace Grand.Plugin.Payments.PayByBitcoin.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
    }
}