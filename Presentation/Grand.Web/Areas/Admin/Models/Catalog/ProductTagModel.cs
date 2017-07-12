using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Catalog;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Catalog
{
    //[Validator(typeof(ProductTagValidator))]
    public partial class ProductTagModel : BaseNopEntityModel, ILocalizedModel<ProductTagLocalizedModel>
    {
        public ProductTagModel()
        {
            Locales = new List<ProductTagLocalizedModel>();
        }
        [GrandResourceDisplayName("Admin.Catalog.ProductTags.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.ProductTags.Fields.ProductCount")]
        public int ProductCount { get; set; }

        public IList<ProductTagLocalizedModel> Locales { get; set; }
    }

    public partial class ProductTagLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.ProductTags.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }
    }
}