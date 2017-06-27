using Grand.Web.Areas.Admin.Models.Common;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Orders
{
    public partial class OrderAddressModel : BaseNopModel
    {
        public string OrderId { get; set; }
        public AddressModel Address { get; set; }
    }
}