using System.Collections.Generic;
using Grand.Web.Areas.Admin.Models.Stores;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Settings
{
    public partial class StoreScopeConfigurationModel : BaseNopModel
    {
        public StoreScopeConfigurationModel()
        {
            Stores = new List<StoreModel>();
        }

        public string StoreId { get; set; }
        public IList<StoreModel> Stores { get; set; }
    }
}