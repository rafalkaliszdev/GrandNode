﻿using FluentValidation.Attributes;
using Grand.Web.Areas.Admin.Models.Stores;
//using Grand.Admin.Validators.Messages;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Web;*/
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grand.Web.Areas.Admin.Models.Messages
{
    //[Validator(typeof(NewsletterCategoryValidator))]
    public partial class NewsletterCategoryModel: BaseNopEntityModel, ILocalizedModel<NewsletterCategoryLocalizedModel>
    {
        public NewsletterCategoryModel()
        {
            Locales = new List<NewsletterCategoryLocalizedModel>();
        }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.Description")]
        /*[AllowHtml]*/
        public string Description { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.Selected")]
        public bool Selected { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        public string[] SelectedStoreIds { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        public IList<NewsletterCategoryLocalizedModel> Locales { get; set; }
    }

    public partial class NewsletterCategoryLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Promotions.NewsletterCategory.Fields.Description")]
        /*[AllowHtml]*/
        public string Description { get; set; }

    }
}