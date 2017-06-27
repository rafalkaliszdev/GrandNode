using System;
/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Forums;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Forums
{
    //[Validator(typeof(ForumGroupValidator))]
    public partial class ForumGroupModel : BaseNopEntityModel
    {
        [GrandResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Forums.ForumGroup.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}