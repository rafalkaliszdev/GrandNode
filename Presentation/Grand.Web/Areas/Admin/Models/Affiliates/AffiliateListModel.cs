﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Affiliates
{
    public partial class AffiliateListModel : BaseNopModel
    {
        [GrandResourceDisplayName("Admin.Affiliates.List.SearchFirstName")]
        /*[AllowHtml]*/
        public string SearchFirstName { get; set; }

        [GrandResourceDisplayName("Admin.Affiliates.List.SearchLastName")]
        /*[AllowHtml]*/
        public string SearchLastName { get; set; }

        [GrandResourceDisplayName("Admin.Affiliates.List.SearchFriendlyUrlName")]
        /*[AllowHtml]*/
        public string SearchFriendlyUrlName { get; set; }

        [GrandResourceDisplayName("Admin.Affiliates.List.LoadOnlyWithOrders")]
        public bool LoadOnlyWithOrders { get; set; }
        [GrandResourceDisplayName("Admin.Affiliates.List.OrdersCreatedFromUtc")]
        [UIHint("DateNullable")]
        public DateTime? OrdersCreatedFromUtc { get; set; }
        [GrandResourceDisplayName("Admin.Affiliates.List.OrdersCreatedToUtc")]
        [UIHint("DateNullable")]
        public DateTime? OrdersCreatedToUtc { get; set; }
    }
}