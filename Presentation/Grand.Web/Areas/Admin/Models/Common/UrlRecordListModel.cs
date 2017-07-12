using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Common
{
    public partial class UrlRecordListModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.System.SeNames.Name")]
        /*[AllowHtml]*/
        public string SeName { get; set; }
    }
}