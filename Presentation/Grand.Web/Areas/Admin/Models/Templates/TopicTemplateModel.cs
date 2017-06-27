/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Templates;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Templates
{
    //[Validator(typeof(TopicTemplateValidator))]
    public partial class TopicTemplateModel : BaseNopEntityModel
    {
        [GrandResourceDisplayName("Admin.System.Templates.Topic.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.System.Templates.Topic.ViewPath")]
        /*[AllowHtml]*/
        public string ViewPath { get; set; }

        [GrandResourceDisplayName("Admin.System.Templates.Topic.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}