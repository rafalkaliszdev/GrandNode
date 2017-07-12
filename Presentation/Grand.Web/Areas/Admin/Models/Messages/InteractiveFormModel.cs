﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Messages;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Grand.Web.Framework.Localization;

namespace Grand.Web.Areas.Admin.Models.Messages
{
    //[Validator(typeof(InteractiveFormValidator))]
    public partial class InteractiveFormModel : BaseNopEntityModel, ILocalizedModel<InteractiveFormLocalizedModel>
    {
        public InteractiveFormModel()
        {
            Locales = new List<InteractiveFormLocalizedModel>();
            AvailableEmailAccounts = new List<EmailAccountModel>();
        }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Name")]
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Body")]
        /*[AllowHtml]*/
        public string Body { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.EmailAccount")]
        public string EmailAccountId { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.AvailableTokens")]
        public string AvailableTokens { get; set; }
        public IList<EmailAccountModel> AvailableEmailAccounts { get; set; }

        public IList<InteractiveFormLocalizedModel> Locales { get; set; }

    }

    public partial class InteractiveFormLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Name")]
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Body")]
        /*[AllowHtml]*/
        public string Body { get; set; }

    }

}