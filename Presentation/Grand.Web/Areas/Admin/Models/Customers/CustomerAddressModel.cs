using Grand.Web.Areas.Admin.Models.Common;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Customers
{
    public partial class CustomerAddressModel : BaseNopModel
    {
        public string CustomerId { get; set; }

        public AddressModel Address { get; set; }
    }
}