/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Admin.Models.Vendors
{
    public partial class VendorListModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.Vendors.List.SearchName")]
        /*[AllowHtml]*/
        public string SearchName { get; set; }
    }
}