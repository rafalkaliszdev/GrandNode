using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Orders
{
    public partial class NeverSoldReportLineModel : BaseNopModel
    {
        public string ProductId { get; set; }
        [GrandResourceDisplayName("Admin.SalesReport.NeverSold.Fields.Name")]
        public string ProductName { get; set; }
    }
}