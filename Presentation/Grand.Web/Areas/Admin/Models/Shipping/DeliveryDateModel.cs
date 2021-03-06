﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Shipping;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Shipping
{
    //[Validator(typeof(DeliveryDateValidator))]
    public partial class DeliveryDateModel : BaseNopEntityModel, ILocalizedModel<DeliveryDateLocalizedModel>
    {
        public DeliveryDateModel()
        {
            Locales = new List<DeliveryDateLocalizedModel>();
        }
        [GrandResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.ColorSquaresRgb")]
        /*[AllowHtml]*/
        public string ColorSquaresRgb { get; set; }

        public IList<DeliveryDateLocalizedModel> Locales { get; set; }
    }

    public partial class DeliveryDateLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

    }
}