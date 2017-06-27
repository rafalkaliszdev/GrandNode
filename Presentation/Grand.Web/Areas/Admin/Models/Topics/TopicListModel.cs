using System.Collections.Generic;
/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Topics
{
    public partial class TopicListModel : BaseNopModel
    {
        public TopicListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [GrandResourceDisplayName("Admin.ContentManagement.Topics.List.SearchStore")]
        public string SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}