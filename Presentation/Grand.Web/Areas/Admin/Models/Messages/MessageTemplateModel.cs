﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
using Grand.Web.Areas.Admin.Models.Stores;
//using Grand.Admin.Validators.Messages;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Messages
{
    //[Validator(typeof(MessageTemplateValidator))]
    public partial class MessageTemplateModel : BaseNopEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<EmailAccountModel>();
            AvailableStores = new List<StoreModel>();
        }


        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        /*[AllowHtml]*/
        public string BccEmailAddresses { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        /*[AllowHtml]*/
        public string Subject { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        /*[AllowHtml]*/
        public string Body { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        /*[AllowHtml]*/
        public bool IsActive { get; set; }
        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend")]
        [UIHint("Int32Nullable")]
        public int? DelayBeforeSend { get; set; }
        public int DelayPeriodId { get; set; }

        public bool HasAttachedDownload { get; set; }
        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public string AttachedDownloadId { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public string EmailAccountId { get; set; }
        public IList<EmailAccountModel> AvailableEmailAccounts { get; set; }

        //Store mapping
        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public string[] SelectedStoreIds { get; set; }
        //comma-separated list of stores used on the list page
        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public string ListOfStores { get; set; }



        public IList<MessageTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class MessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        /*[AllowHtml]*/
        public string BccEmailAddresses { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        /*[AllowHtml]*/
        public string Subject { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        /*[AllowHtml]*/
        public string Body { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public string EmailAccountId { get; set; }
    }
}