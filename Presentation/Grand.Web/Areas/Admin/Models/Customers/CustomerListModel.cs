﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Grand.Web.Areas.Admin.Models.Customers
{
    public partial class CustomerListModel : BaseNopModel
    {
        public CustomerListModel()
        {
            AvailableCustomerTags = new List<SelectListItem>();
            SearchCustomerTagIds = new List<string>();
            SearchCustomerRoleIds = new List<string>();
            AvailableCustomerRoles = new List<SelectListItem>();
        }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        /*[AllowHtml]*/
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }


        [GrandResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<string> SearchCustomerRoleIds { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.CustomerTags")]
        public IList<SelectListItem> AvailableCustomerTags { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.CustomerTags")]
        [UIHint("MultiSelect")]
        public IList<string> SearchCustomerTagIds { get; set; }


        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        /*[AllowHtml]*/
        public string SearchEmail { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchUsername")]
        /*[AllowHtml]*/
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchFirstName")]
        /*[AllowHtml]*/
        public string SearchFirstName { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchLastName")]
        /*[AllowHtml]*/
        public string SearchLastName { get; set; }


        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchCompany")]
        /*[AllowHtml]*/
        public string SearchCompany { get; set; }
        public bool CompanyEnabled { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchPhone")]
        /*[AllowHtml]*/
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.List.SearchZipCode")]
        /*[AllowHtml]*/
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }
    }
}