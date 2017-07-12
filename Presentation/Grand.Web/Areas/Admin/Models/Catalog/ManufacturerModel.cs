﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
using Grand.Web.Areas.Admin.Models.Customers;
using Grand.Web.Areas.Admin.Models.Discounts;
using Grand.Web.Areas.Admin.Models.Stores;
//using Grand.Admin.Validators.Catalog;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;
using System;

namespace Grand.Web.Areas.Admin.Models.Catalog
{
    //[Validator(typeof(ManufacturerValidator))]
    public partial class ManufacturerModel : BaseNopEntityModel, ILocalizedModel<ManufacturerLocalizedModel>
    {
        public ManufacturerModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<ManufacturerLocalizedModel>();
            AvailableManufacturerTemplates = new List<SelectListItem>();
        }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Description")]
        /*[AllowHtml]*/
        public string Description { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.ManufacturerTemplate")]
        public string ManufacturerTemplateId { get; set; }
        public IList<SelectListItem> AvailableManufacturerTemplates { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaKeywords")]
        /*[AllowHtml]*/
        public string MetaKeywords { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaDescription")]
        /*[AllowHtml]*/
        public string MetaDescription { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaTitle")]
        /*[AllowHtml]*/
        public string MetaTitle { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SeName")]
        /*[AllowHtml]*/
        public string SeName { get; set; }

        [UIHint("Picture")]
        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Picture")]
        public string PictureId { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PageSize")]
        public int PageSize { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceRanges")]
        /*[AllowHtml]*/
        public string PriceRanges { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Published")]
        public bool Published { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Deleted")]
        public bool Deleted { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        
        public IList<ManufacturerLocalizedModel> Locales { get; set; }
        
        //ACL
        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SubjectToAcl")]
        public bool SubjectToAcl { get; set; }
        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AclCustomerRoles")]
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public string[] SelectedCustomerRoleIds { get; set; }

        //Store mapping
        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public string[] SelectedStoreIds { get; set; }


        //discounts
        public List<DiscountModel> AvailableDiscounts { get; set; }
        public string[] SelectedDiscountIds { get; set; }


        #region Nested classes

        public partial class ManufacturerProductModel : BaseNopEntityModel
        {
            public string ManufacturerId { get; set; }

            public string ProductId { get; set; }

            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.Product")]
            public string ProductName { get; set; }

            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.IsFeaturedProduct")]
            public bool IsFeaturedProduct { get; set; }

            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class AddManufacturerProductModel : BaseNopModel
        {
            public AddManufacturerProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [GrandResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            /*[AllowHtml]*/
            public string SearchProductName { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public string SearchCategoryId { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public string SearchManufacturerId { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public string SearchStoreId { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public string SearchVendorId { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public string ManufacturerId { get; set; }

            public string[] SelectedProductIds { get; set; }
        }

        public partial class ActivityLogModel : BaseNopEntityModel
        {
            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.ActivityLog.ActivityLogType")]
            public string ActivityLogTypeName { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.ActivityLog.Comment")]
            public string Comment { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.ActivityLog.CreatedOn")]
            public DateTime CreatedOn { get; set; }
            [GrandResourceDisplayName("Admin.Catalog.Manufacturers.ActivityLog.Customer")]
            public string CustomerId { get; set; }
            public string CustomerEmail { get; set; }
        }



        #endregion
    }

    public partial class ManufacturerLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Description")]
        /*[AllowHtml]*/
        public string Description {get;set;}

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaKeywords")]
        /*[AllowHtml]*/
        public string MetaKeywords { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaDescription")]
        /*[AllowHtml]*/
        public string MetaDescription { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaTitle")]
        /*[AllowHtml]*/
        public string MetaTitle { get; set; }

        [GrandResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SeName")]
        /*[AllowHtml]*/
        public string SeName { get; set; }
    }
}