﻿using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Web;*/

namespace Grand.Web.Areas.Admin.Models.Orders
{
    public partial class OrderPeriodReportLineModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.SalesReport.Period.Name")]
        public string Period { get; set; }

        [GrandResourceDisplayName("Admin.SalesReport.Period.Count")]
        public int Count { get; set; }

        [GrandResourceDisplayName("Admin.SalesReport.Period.Amount")]
        public decimal Amount { get; set; }

    }
}