using Microsoft.AspNetCore.Mvc.Rendering;
/*using System.Web.Routing;*/
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Grand.Web.Areas.Admin.Models.Payments
{
    public partial class PaymentMethodModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.FriendlyName")]
        /*[AllowHtml]*/
        public string FriendlyName { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SystemName")]
        /*[AllowHtml]*/
        public string SystemName { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.IsActive")]
        public bool IsActive { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.Logo")]
        public string LogoUrl { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportCapture")]
        public bool SupportCapture { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportPartiallyRefund")]
        public bool SupportPartiallyRefund { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportRefund")]
        public bool SupportRefund { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SupportVoid")]
        public bool SupportVoid { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.RecurringPaymentType")]
        public string RecurringPaymentType { get; set; }
        



        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}