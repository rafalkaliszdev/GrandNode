﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Customers;
using Grand.Core.Domain.Catalog;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Customers
{
    //[Validator(typeof(CustomerValidator))]
    public partial class CustomerModel : BaseNopEntityModel
    {
        public CustomerModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.SendEmail = new SendEmailModel() { SendImmediately = true }; 
            this.SendPm = new SendPmModel();
            this.AvailableCustomerRoles = new List<CustomerRoleModel>();
            this.AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.AvailableVendors = new List<SelectListItem>();
            this.CustomerAttributes = new List<CustomerAttributeModel>();
            this.AvailableNewsletterSubscriptionStores = new List<StoreModel>();
            this.RewardPointsAvailableStores = new List<SelectListItem>();
        }

        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
        /*[AllowHtml]*/
        public string Username { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
        /*[AllowHtml]*/
        public string Email { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Password")]
        /*[AllowHtml]*/
        [DataType(DataType.Password)]
        [NoTrim]
        public string Password { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.CustomerTags")]
        public string CustomerTags { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Vendor")]
        public string VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
        public string Gender { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.FirstName")]
        /*[AllowHtml]*/
        public string FirstName { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.LastName")]
        /*[AllowHtml]*/
        public string LastName { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }
        
        public bool DateOfBirthEnabled { get; set; }
        [UIHint("DateNullable")]
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Company")]
        /*[AllowHtml]*/
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress")]
        /*[AllowHtml]*/
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress2")]
        /*[AllowHtml]*/
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.ZipPostalCode")]
        /*[AllowHtml]*/
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.City")]
        /*[AllowHtml]*/
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Country")]
        public string CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.StateProvince")]
        public string StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Phone")]
        /*[AllowHtml]*/
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Fax")]
        /*[AllowHtml]*/
        public string Fax { get; set; }

        public List<CustomerAttributeModel> CustomerAttributes { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.AdminComment")]
        /*[AllowHtml]*/
        public string AdminComment { get; set; }
        
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.FreeShipping")]
        public bool FreeShipping { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Active")]
        public bool Active { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
        public string AffiliateId { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
        public string AffiliateName { get; set; }

        //time zone
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
        /*[AllowHtml]*/
        public string TimeZoneId { get; set; }

        public bool AllowCustomersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.VatNumber")]
        /*[AllowHtml]*/
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }

        //registration date
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.LastPurchaseDate")]
        public DateTime? LastPurchaseDate { get; set; }

        //IP adderss
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }


        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.LastUrlReferrer")]
        public string LastUrlReferrer { get; set; }

        //customer roles
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }
        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }
        public string[] SelectedCustomerRoleIds { get; set; }

        //newsletter subscriptions (per store)
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Newsletter")]
        public List<StoreModel> AvailableNewsletterSubscriptionStores { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.Fields.Newsletter")]
        public string[] SelectedNewsletterSubscriptionStoreIds { get; set; }

        //reward points history
        public bool DisplayRewardPointsHistory { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue")]
        public int AddRewardPointsValue { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsMessage")]
        /*[AllowHtml]*/
        public string AddRewardPointsMessage { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore")]
        public string AddRewardPointsStoreId { get; set; }
        [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore")]
        public IList<SelectListItem> RewardPointsAvailableStores { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }
        //send PM model
        public SendPmModel SendPm { get; set; }
        //send the welcome message
        public bool AllowSendingOfWelcomeMessage { get; set; }
        //re-send the activation message
        public bool AllowReSendingOfActivationMessage { get; set; }
        public bool ShowMessageContactForm { get; set; }

        [GrandResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }


        #region Nested classes

        public partial class StoreModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }

        public partial class AssociatedExternalAuthModel : BaseNopEntityModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.Email")]
            public string Email { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.ExternalIdentifier")]
            public string ExternalIdentifier { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.AuthMethodName")]
            public string AuthMethodName { get; set; }
        }

        public partial class RewardPointsHistoryModel : BaseNopEntityModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Store")]
            public string StoreName { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.PointsBalance")]
            public int PointsBalance { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Message")]
            /*[AllowHtml]*/
            public string Message { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class SendEmailModel : BaseNopModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.SendEmail.Subject")]
            /*[AllowHtml]*/
            public string Subject { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.SendEmail.Body")]
            /*[AllowHtml]*/
            public string Body { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.SendEmail.SendImmediately")]
            public bool SendImmediately { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.SendEmail.DontSendBeforeDate")]
            [UIHint("DateTimeNullable")]
            public DateTime? DontSendBeforeDate { get; set; }
        }

        public partial class SendPmModel : BaseNopModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.SendPM.Subject")]
            public string Subject { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial class OrderModel : BaseNopEntityModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.ID")]
            public override string Id { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.ID")]
            public int OrderNumber { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.OrderStatus")]
            public string OrderStatus { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.Store")]
            public string StoreName { get; set; }

            [GrandResourceDisplayName("Admin.Customers.Customers.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class ActivityLogModel : BaseNopEntityModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.ActivityLog.ActivityLogType")]
            public string ActivityLogTypeName { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.ActivityLog.Comment")]
            public string Comment { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.ActivityLog.CreatedOn")]
            public DateTime CreatedOn { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.ActivityLog.IpAddress")]
            public string IpAddress { get; set; }
        }

        public partial class BackInStockSubscriptionModel : BaseNopEntityModel
        {
            [GrandResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.Store")]
            public string StoreName { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.Product")]
            public string ProductId { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.Product")]
            public string ProductName { get; set; }
            [GrandResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class CustomerAttributeModel : BaseNopEntityModel
        {
            public CustomerAttributeModel()
            {
                Values = new List<CustomerAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CustomerAttributeValueModel> Values { get; set; }

        }

        public partial class CustomerAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}