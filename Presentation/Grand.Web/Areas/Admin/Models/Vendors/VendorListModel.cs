using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Vendors
{
    public partial class VendorListModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.Vendors.List.SearchName")]
        /*[AllowHtml]*/
        public string SearchName { get; set; }
    }
}