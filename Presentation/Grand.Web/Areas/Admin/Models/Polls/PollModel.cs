﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Polls;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Grand.Web.Framework.Localization;
using Grand.Web.Areas.Admin.Models.Stores;
using System.Collections.Generic;
using Grand.Web.Areas.Admin.Models.Customers;

namespace Grand.Web.Areas.Admin.Models.Polls
{
    //[Validator(typeof(PollValidator))]
    public partial class PollModel : BaseNopEntityModel, ILocalizedModel<PollLocalizedModel>
    {

        public PollModel()
        {
            AvailableStores = new List<StoreModel>();
            Locales = new List<PollLocalizedModel>();
            AvailableCustomerRoles = new List<CustomerRoleModel>();
        }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.SystemKeyword")]
        /*[AllowHtml]*/
        public string SystemKeyword { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.Published")]
        public bool Published { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.AllowGuestsToVote")]
        public bool AllowGuestsToVote { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.StartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDate { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDate { get; set; }

        //Store mapping
        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.AvailableStores")]
        public IList<StoreModel> AvailableStores { get; set; }
        public string[] SelectedStoreIds { get; set; }
        public IList<PollLocalizedModel> Locales { get; set; }


        //ACL
        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }
        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public string[] SelectedCustomerRoleIds { get; set; }

    }

    public partial class PollLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Fields.Name")]
        public string Name { get; set; }

    }

}