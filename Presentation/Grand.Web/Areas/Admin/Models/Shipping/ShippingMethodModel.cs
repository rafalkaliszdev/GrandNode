using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Shipping;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Shipping
{
    //[Validator(typeof(ShippingMethodValidator))]
    public partial class ShippingMethodModel : BaseNopEntityModel, ILocalizedModel<ShippingMethodLocalizedModel>
    {
        public ShippingMethodModel()
        {
            Locales = new List<ShippingMethodLocalizedModel>();
        }
        [GrandResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Description")]
        /*[AllowHtml]*/
        public string Description { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ShippingMethodLocalizedModel> Locales { get; set; }
    }

    public partial class ShippingMethodLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Description")]
        /*[AllowHtml]*/
        public string Description { get; set; }

    }
}