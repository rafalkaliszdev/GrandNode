/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Templates;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Templates
{
    //[Validator(typeof(ManufacturerTemplateValidator))]
    public partial class ManufacturerTemplateModel : BaseNopEntityModel
    {
        [GrandResourceDisplayName("Admin.System.Templates.Manufacturer.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.System.Templates.Manufacturer.ViewPath")]
        /*[AllowHtml]*/
        public string ViewPath { get; set; }

        [GrandResourceDisplayName("Admin.System.Templates.Manufacturer.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}