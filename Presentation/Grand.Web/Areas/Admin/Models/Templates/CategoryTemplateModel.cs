using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Templates;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Templates
{
    //[Validator(typeof(CategoryTemplateValidator))]
    public partial class CategoryTemplateModel : BaseNopEntityModel
    {
        [GrandResourceDisplayName("Admin.System.Templates.Category.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.System.Templates.Category.ViewPath")]
        /*[AllowHtml]*/
        public string ViewPath { get; set; }

        [GrandResourceDisplayName("Admin.System.Templates.Category.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}