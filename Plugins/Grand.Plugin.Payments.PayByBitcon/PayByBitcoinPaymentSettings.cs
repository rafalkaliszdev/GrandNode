using Grand.Core.Configuration;

namespace Grand.Plugin.Payments.PayByBitcoin
{
    public class PayByBitcoinPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
    }
}
