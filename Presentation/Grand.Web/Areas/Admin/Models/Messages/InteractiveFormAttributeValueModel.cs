﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Messages;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Grand.Web.Framework.Localization;
using System.ComponentModel.DataAnnotations;

namespace Grand.Web.Areas.Admin.Models.Messages
{
    //[Validator(typeof(InteractiveFormAttributeValueValidator))]
    public partial class InteractiveFormAttributeValueModel : BaseNopEntityModel, ILocalizedModel<InteractiveFormAttributeValueLocalizedModel>
    {
        public InteractiveFormAttributeValueModel()
        {
            Locales = new List<InteractiveFormAttributeValueLocalizedModel>();
        }
        public string FormId { get; set; }
        public string AttributeId { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Values.Fields.Name")]
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        public IList<InteractiveFormAttributeValueLocalizedModel> Locales { get; set; }

    }

    public partial class InteractiveFormAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Values.Fields.Name")]
        public string Name { get; set; }

    }

}