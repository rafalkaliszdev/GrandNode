using System;
using System.Linq;
///*using System.Web;*/
using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Directory;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Tax;
using Grand.Core.Domain.Vendors;
using Grand.Core.Fakes;
using Grand.Services.Authentication;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Stores;
using Grand.Services.Vendors;
using Grand.Web.Framework.Localization;
using Grand.Core.Domain.Common;
using Grand.Core.Data;

//cholera nie wykrywa biblioteki.
using Microsoft.AspNetCore.Http;

namespace Grand.Web.Framework
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Const

        private const string CustomerCookieName = "NopCookie";//"grand.customer";

        #endregion

        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly IStoreContext _storeContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IStoreMappingService _storeMappingService;
        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private Vendor _cachedVendor;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        public WebWorkContext(
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            IVendorService vendorService,
            IStoreContext storeContext,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            LocalizationSettings localizationSettings,
            IUserAgentHelper userAgentHelper,
            IStoreMappingService storeMappingService
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            this._customerService = customerService;
            this._vendorService = vendorService;
            this._storeContext = storeContext;
            this._authenticationService = authenticationService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
            this._localizationSettings = localizationSettings;
            this._userAgentHelper = userAgentHelper;
            this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Utilities

        protected virtual /*HttpCookie*/string GetCustomerCookie()
        {

            //tbh
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext.Request == null)
                return null;



            var cookie1 = _httpContextAccessor.HttpContext.Request.Cookies["grand.customer"];//yeah it is non null 2017_06_01 14:28
            var cookie2 = _httpContextAccessor.HttpContext.Request.Cookies["NopCookie"];
            var cookie3 = _httpContextAccessor.HttpContext.Request.Cookies["nopCommerce"];


            

            return _httpContextAccessor.HttpContext.Request.Cookies[CustomerCookieName];
        }

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext.Response != null)
            {
                var cookie = /*new HttpCookie(*/CustomerCookieName;
                //cookie.HttpOnly = true;
                cookie = customerGuid.ToString();
                if (customerGuid == Guid.Empty)
                {
                    //cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    //cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                //tbh
                //2017_04_25 15:14
                //seriously nie mam pojecia czy to co tearz robie jest poprawne, ale to podpowiada mi logika


                _httpContextAccessor.HttpContext.Response.Cookies.Delete(CustomerCookieName);
                _httpContextAccessor.HttpContext.Response.Cookies.Append(CustomerCookieName, cookie);//Remove(CustomerCookieName);
            }
        }

        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext.Request == null)
                return null;

            //tbh
            string virtualPath = _httpContextAccessor.HttpContext.Request.Path;// AppRelativeCurrentExecutionFilePath;
            string applicationPath = _httpContextAccessor.HttpContext.Request.PathBase;//.ApplicationPath;
            if (!virtualPath.IsLocalizedUrl(applicationPath, false))
                return null;

            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
            if (String.IsNullOrEmpty(seoCode))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.OrdinalIgnoreCase));
            if (language != null && language.Published && _storeMappingService.Authorize(language))
            {
                return language;
            }

            return null;
        }

        protected virtual Language GetLanguageFromBrowserSettings()
        {
            if (_httpContextAccessor == null ||
                _httpContextAccessor.HttpContext.Request == null ||
                _httpContextAccessor.HttpContext.Request/*UserLanguages*/ == null)
                return null;


            //tbh
            //var userLanguage = _httpContextAccessor.HttpContext.Request.UserLanguages.FirstOrDefault();
            //if (String.IsNullOrEmpty(userLanguage))
            //    return null;


            //tbh
            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault();// (l => userLanguage.Equals(l.LanguageCulture, StringComparison.OrdinalIgnoreCase));
            if (language != null && language.Published && _storeMappingService.Authorize(language))
            {
                return language;
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual Customer CurrentCustomer
        {
            get
            {
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;
                if (_httpContextAccessor == null || _httpContextAccessor is FakeHttpContext)
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                }

                //check whether request is made by a search engine or webapi
                //in this case return built-in customer record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                if (false) // customer == null || customer.Deleted || !customer.Active)
                {
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
                    else
                        if (_userAgentHelper.IsWebApi())
                    {
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.WebApi);
                    }
                }

                //registered user
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }

                //impersonate user if required (currently used for 'phone order' support)
                if (customer != null && !customer.Deleted && customer.Active)
                {
                    var impersonatedCustomerId = customer.GetAttribute<string>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                    if (!String.IsNullOrEmpty(impersonatedCustomerId))
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                //load guest customer
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    //tbh
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie/*.Value*/))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie/*.Value*/, out customerGuid))
                        {
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null &&
                                //this customer (from cookie) should not be registered
                                !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _customerService.InsertGuestCustomer();
                }


                //validation (refreshment of cookie)
                if (!customer.Deleted && customer.Active)
                {
                    SetCustomerCookie(customer.CustomerGuid);
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        public virtual Customer OriginalCustomerIfImpersonated
        {
            get
            {
                return _originalCustomerIfImpersonated;
            }
        }

        /// <summary>
        /// Gets or sets the current vendor (logged-in manager)
        /// </summary>
        public virtual Vendor CurrentVendor
        {
            get
            {
                if (_cachedVendor != null)
                    return _cachedVendor;

                var currentCustomer = this.CurrentCustomer;
                if (currentCustomer == null)
                    return null;

                var vendor = _vendorService.GetVendorById(currentCustomer.VendorId);

                //validation
                if (vendor != null && !vendor.Deleted && vendor.Active)
                    _cachedVendor = vendor;

                return _cachedVendor;
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                //Language detectedLanguage = null;
                //if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                //{
                //    //get language from URL
                //    detectedLanguage = GetLanguageFromUrl();
                //}
                //if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                //{
                //    //get language from browser settings
                //    //but we do it only once
                //    if (!this.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.LanguageAutomaticallyDetected, _storeContext.CurrentStore.Id))
                //    {
                //        detectedLanguage = GetLanguageFromBrowserSettings();
                //        if (detectedLanguage != null)
                //        {
                //            _genericAttributeService.SaveAttribute(this.CurrentCustomer, SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                //                 true, _storeContext.CurrentStore.Id);
                //            //this.CurrentCustomer.GenericAttributes = _customerService.GetCustomerById(this.CurrentCustomer.Id).GenericAttributes;
                //        }
                //    }
                //}
                //if (detectedLanguage != null)
                //{
                //    //the language is detected. now we need to save it
                //    if (this.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LanguageId, _storeContext.CurrentStore.Id) != detectedLanguage.Id)
                //    {
                //        _genericAttributeService.SaveAttribute(this.CurrentCustomer, SystemCustomerAttributeNames.LanguageId,
                //            detectedLanguage.Id, _storeContext.CurrentStore.Id);
                //        //this.CurrentCustomer.GenericAttributes = _customerService.GetCustomerById(this.CurrentCustomer.Id).GenericAttributes;
                //    }
                //}

                //var allLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);
                ////find current customer language
                //var languageId = _genericAttributeService.GetAttributesForEntity<string>(this.CurrentCustomer, SystemCustomerAttributeNames.LanguageId, _storeContext.CurrentStore.Id);
                ////var languageId = this.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.LanguageId, _storeContext.CurrentStore.Id);
                //var language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                //if (language == null)
                //{
                //    //it not specified, then return the first (filtered by current store) found one
                //    languageId = _storeContext.CurrentStore.DefaultLanguageId;
                //    language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                //}

                Language language = null;
                if (language == null)
                {
                    //it not specified, then return the first found one
                    language = _languageService.GetAllLanguages().FirstOrDefault();
                }

                //cache
                _cachedLanguage = language;
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : "";
                _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                    SystemCustomerAttributeNames.LanguageId,
                    languageId, _storeContext.CurrentStore.Id);
                //this.CurrentCustomer.GenericAttributes = _customerService.GetCustomerById(this.CurrentCustomer.Id).GenericAttributes;

                //reset cache
                _cachedLanguage = null;
            }
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        public virtual Currency WorkingCurrency
        {
            get
            {
                if (_cachedCurrency != null)
                    return _cachedCurrency;

                ////return primary store currency when we're in admin area/mode
                //if (this.IsAdmin)
                //{
                //    var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                //    if (primaryStoreCurrency != null)
                //    {
                //        //cache
                //        _cachedCurrency = primaryStoreCurrency;
                //        return primaryStoreCurrency;
                //    }
                //}

                //var allCurrencies = _currencyService.GetAllCurrencies();// storeId: _storeContext.CurrentStore.Id);
                ////find a currency previously selected by a customer
                //var currencyId = this.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CurrencyId, _storeContext.CurrentStore.Id);
                //var currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);
                //if (currency == null)
                //{
                //    //it not found, then let's load the default currency for the current language (if specified)
                //    currencyId = this.WorkingLanguage.DefaultCurrencyId;
                //    currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);
                //}
                //if (currency == null)
                //{
                //    //it not found, then return the first (filtered by current store) found one
                //    currency = allCurrencies.FirstOrDefault();
                //}
                Currency currency = null;
                if (currency == null)
                {
                    //it not specified, then return the first found one
                    currency = _currencyService.GetAllCurrencies().FirstOrDefault();
                }

                //cache
                _cachedCurrency = currency;
                return _cachedCurrency;
            }
            set
            {
                var currencyId = value != null ? value.Id : "";
                _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                    SystemCustomerAttributeNames.CurrencyId,
                    currencyId, _storeContext.CurrentStore.Id);
                //this.CurrentCustomer.GenericAttributes = _customerService.GetCustomerById(this.CurrentCustomer.Id).GenericAttributes;

                //reset cache
                _cachedCurrency = null;
            }
        }

        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        public virtual TaxDisplayType TaxDisplayType
        {
            get
            {
                //cache
                if (_cachedTaxDisplayType != null)
                    return _cachedTaxDisplayType.Value;

                TaxDisplayType taxDisplayType;
                if (_taxSettings.AllowCustomersToSelectTaxDisplayType && this.CurrentCustomer != null)
                {
                    taxDisplayType = (TaxDisplayType)this.CurrentCustomer.GetAttribute<int>(
                        SystemCustomerAttributeNames.TaxDisplayTypeId, _storeContext.CurrentStore.Id);
                }
                else
                {
                    taxDisplayType = _taxSettings.TaxDisplayType;
                }

                //cache
                _cachedTaxDisplayType = taxDisplayType;
                return _cachedTaxDisplayType.Value;

            }
            set
            {
                if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                    return;

                _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                    SystemCustomerAttributeNames.TaxDisplayTypeId,
                    (int)value, _storeContext.CurrentStore.Id);
                //this.CurrentCustomer.GenericAttributes = _customerService.GetCustomerById(this.CurrentCustomer.Id).GenericAttributes;
                //reset cache
                _cachedTaxDisplayType = null;

            }
        }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}
