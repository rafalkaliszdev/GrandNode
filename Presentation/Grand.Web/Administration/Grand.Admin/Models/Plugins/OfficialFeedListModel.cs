﻿using System.Collections.Generic;
/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Admin.Models.Plugins
{
    public partial class OfficialFeedListModel : BaseNopModel
    {
        public OfficialFeedListModel()
        {
            AvailableVersions = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailablePrices = new List<SelectListItem>();
        }

        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Name")]
        /*[AllowHtml]*/
        public string SearchName { get; set; }
        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Version")]
        public int SearchVersionId { get; set; }
        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Category")]
        public string SearchCategoryId { get; set; }
        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Price")]
        public int SearchPriceId { get; set; }


        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Version")]
        public IList<SelectListItem> AvailableVersions { get; set; }
        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Category")]
        public IList<SelectListItem> AvailableCategories { get; set; }
        [GrandResourceDisplayName("Admin.Configuration.Plugins.OfficialFeed.Price")]
        public IList<SelectListItem> AvailablePrices { get; set; }

        #region Nested classes

        public partial class ItemOverview
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string SupportedVersions { get; set; }
            public string PictureUrl { get; set; }
            public string Price { get; set; }
        }

        #endregion
    }
}