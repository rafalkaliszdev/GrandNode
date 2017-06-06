﻿using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Vendors;
using Grand.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Web;*/
///*using System.Web.Mvc;*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Grand.Web.Services
{
    public partial interface ICommonWebService
    {
        LogoModel PrepareLogo();
        LanguageSelectorModel PrepareLanguageSelector();
        void SetLanguage(string langid);
        CurrencySelectorModel PrepareCurrencySelector();
        void SetCurrency(string customerCurrency);
        TaxTypeSelectorModel PrepareTaxTypeSelector();
        void SetTaxType(int customerTaxType);
        int GetUnreadPrivateMessages();
        HeaderLinksModel PrepareHeaderLinks(Customer customer);
        AdminHeaderLinksModel PrepareAdminHeaderLinks(Customer customer);
        Task<HeaderLinksModel> PrepareHeaderLinksAsync(Customer customer);
        FooterModel PrepareFooter();
        ContactUsModel PrepareContactUs();
        ContactUsModel SendContactUs(ContactUsModel model);
        ContactVendorModel PrepareContactVendor(Vendor vendor);
        ContactVendorModel SendContactVendor(ContactVendorModel model, Vendor vendor);
        SitemapModel PrepareSitemap();
        string SitemapXml(int? id, UrlHelper url);
        StoreThemeSelectorModel PrepareStoreThemeSelector();
        FaviconModel PrepareFavicon();
        string PrepareRobotsTextFile();

    }
}