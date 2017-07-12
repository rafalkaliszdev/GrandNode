﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Common;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Common
{
    //[Validator(typeof(AddressAttributeValueValidator))]
    public partial class AddressAttributeValueModel : BaseNopEntityModel, ILocalizedModel<AddressAttributeValueLocalizedModel>
    {
        public AddressAttributeValueModel()
        {
            Locales = new List<AddressAttributeValueLocalizedModel>();
        }

        public string AddressAttributeId { get; set; }

        [GrandResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [GrandResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<AddressAttributeValueLocalizedModel> Locales { get; set; }

    }

    public partial class AddressAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }
    }
}