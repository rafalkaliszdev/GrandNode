/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Catalog
{
    public partial class CopyProductModel : BaseNopEntityModel
    {

        [GrandResourceDisplayName("Admin.Catalog.Products.Copy.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Products.Copy.CopyImages")]
        public bool CopyImages { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Products.Copy.Published")]
        public bool Published { get; set; }

    }
}