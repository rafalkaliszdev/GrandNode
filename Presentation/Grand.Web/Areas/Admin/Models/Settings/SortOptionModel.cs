using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Settings
{
    public partial class SortOptionModel
    {
        public virtual int Id { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.IsActive")]        
        public bool IsActive { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}