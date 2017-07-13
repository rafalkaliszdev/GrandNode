using System;
using System.Linq;

using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain;
using Grand.Core.Domain.Blogs;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Forums;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Messages;
using Grand.Core.Domain.News;
using Grand.Core.Domain.Tax;
using Grand.Core.Domain.Vendors;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Forums;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Media;
using Grand.Services.Messages;
using Grand.Services.Orders;
using Grand.Services.Security;
using Grand.Services.Seo;
using Grand.Services.Topics;
using Grand.Services.Vendors;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Security;
using Grand.Web.Framework.Security.Captcha;
using Grand.Web.Framework.Themes;
using Grand.Web.Models.Common;
using Grand.Web.Framework.UI;
using System.Text.RegularExpressions;
using Grand.Web.Services;
using Grand.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Vendors;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Services.Vendors;
using Grand.Web.Framework.Security;
using Grand.Web.Models.Catalog;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Grand.Core.Infrastructure;
using Grand.Web.Framework.Themes;
using Grand.Web.Framework.UI;

//using static Autofac.ResolutionExtensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Vendors;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Services.Vendors;
using Grand.Web.Framework.Security;
using Grand.Web.Models.Catalog;
using Grand.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Grand.Web.ViewComponents
{
    public class CommonViewComponent : ViewComponent
    {
        #region Fields
        private readonly ICommonWebService _commonWebService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerActionEventService _customerActionEventService;
        private readonly IPopupService _popupService;
        private readonly IInteractiveFormService _interactiveFormService;

        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Constructors

        public CommonViewComponent(
            ICommonWebService commonWebService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerActivityService customerActivityService,
            ICustomerActionEventService customerActionEventService,
            IPopupService popupService,
            IInteractiveFormService interactiveFormService,

            StoreInformationSettings storeInformationSettings,
            CommonSettings commonSettings,
            ForumSettings forumSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            VendorSettings vendorSettings
            )
        {
            this._commonWebService = commonWebService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerActivityService = customerActivityService;
            this._customerActionEventService = customerActionEventService;
            this._popupService = popupService;
            this._interactiveFormService = interactiveFormService;

            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            if (actionName == nameof(this.HeaderLinks))
                return await HeaderLinks();


            return Content("sometimes you run so fast, you lose your aim out of sight");
        }

        ////logo
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> Logo()
        //{
        //    var model = _commonWebService.PrepareLogo();
        //    return Partial(model);
        //}

        ////language
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> LanguageSelector()
        //{
        //    var model = _commonWebService.PrepareLanguageSelector();
        //    if (model.AvailableLanguages.Count == 1)
        //        Content("");

        //    return PartialView(model);
        //}

        ////currency
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> CurrencySelector()
        //{
        //    var model = _commonWebService.PrepareCurrencySelector();
        //    if (model.AvailableCurrencies.Count == 1)
        //        Content("");

        //    return PartialView(model);
        //}
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual Task<IViewComponentResult> SetCurrency(string customerCurrency, string returnUrl = "")
        //{
        //    _commonWebService.SetCurrency(customerCurrency);

        //    //home page
        //    if (String.IsNullOrEmpty(returnUrl))
        //        returnUrl = Url.RouteUrl("HomePage");

        //    //prevent open redirection attack
        //    if (!Url.IsLocalUrl(returnUrl))
        //        returnUrl = Url.RouteUrl("HomePage");

        //    return Redirect(returnUrl);
        //}

        ////tax type
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> TaxTypeSelector()
        //{
        //    var model = _commonWebService.PrepareTaxTypeSelector();
        //    if (model == null)
        //        return Content("");

        //    return PartialView(model);
        //}
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual Task<IViewComponentResult> SetTaxType(int customerTaxType, string returnUrl = "")
        //{
        //    _commonWebService.SetTaxType(customerTaxType);

        //    //home page
        //    if (String.IsNullOrEmpty(returnUrl))
        //        returnUrl = Url.RouteUrl("HomePage");

        //    //prevent open redirection attack
        //    if (!Url.IsLocalUrl(returnUrl))
        //        returnUrl = Url.RouteUrl("HomePage");

        //    return Redirect(returnUrl);
        //}

        ////footer
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> JavaScriptDisabledWarning()
        //{
        //    if (!_commonSettings.DisplayJavaScriptDisabledWarning)
        //        return Content("");

        //    return PartialView();
        //}

        //header links
        //[ChildActionOnly]
        public virtual async Task<IViewComponentResult> HeaderLinks()
        {
            var customer = _workContext.CurrentCustomer;

            var unreadMessageCount = _commonWebService.GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage =  string.Format(_localizationService.GetResource("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !customer.GetAttribute<bool>(SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages, _storeContext.CurrentStore.Id))
                {
                    EngineContextExperimental.Current.Resolve<IGenericAttributeService>().SaveAttribute(customer, SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages, true, _storeContext.CurrentStore.Id);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            //standard
            //var model = _commonWebService.PrepareHeaderLinks(customer);
            //model.UnreadPrivateMessages = unreadMessage;
            //model.AlertMessage = alertMessage;

            //async
            var modelAsync = await _commonWebService.PrepareHeaderLinksAsync(customer);
            modelAsync.UnreadPrivateMessages = unreadMessage;
            modelAsync.AlertMessage = alertMessage;


            return View("/Views/Shared/Components/Common/HeaderLinks.cshtml", modelAsync);//Views/Shared/Components/Product/

        }
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> AdminHeaderLinks()
        //{
        //    var model = _commonWebService.PrepareAdminHeaderLinks(_workContext.CurrentCustomer);
        //    return PartialView(model);
        //}

        ////footer
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> Footer()
        //{
        //    var model = _commonWebService.PrepareFooter();
        //    return PartialView(model);
        //}

        ////store theme
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> StoreThemeSelector()
        //{
        //    if (!_storeInformationSettings.AllowCustomerToSelectTheme)
        //        return Content("");
        //    var model = _commonWebService.PrepareStoreThemeSelector();
        //    return PartialView(model);
        //}
        //public virtual Task<IViewComponentResult> SetStoreTheme(string themeName, string returnUrl = "")
        //{
        //    EngineContextExperimental.Current.Resolve<IThemeContext>().WorkingThemeName = themeName;

        //    //home page
        //    if (String.IsNullOrEmpty(returnUrl))
        //        returnUrl = Url.RouteUrl("HomePage");

        //    //prevent open redirection attack
        //    if (!Url.IsLocalUrl(returnUrl))
        //        returnUrl = Url.RouteUrl("HomePage");

        //    return Redirect(returnUrl);
        //}

        ////favicon
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> Favicon()
        //{
        //    //try loading a store specific favicon
        //    var model = _commonWebService.PrepareFavicon();
        //    if (String.IsNullOrEmpty(model.FaviconUrl))
        //        return Content("");

        //    return PartialView(model);
        //}

        ////EU Cookie law
        ////[ChildActionOnly]
        //public virtual Task<IViewComponentResult> EuCookieLaw()
        //{
        //    if (!_storeInformationSettings.DisplayEuCookieLawWarning)
        //        //disabled
        //        return Content("");

        //    //ignore search engines because some pages could be indexed with the EU cookie as description
        //    if (_workContext.CurrentCustomer.IsSearchEngineAccount())
        //        return Content("");

        //    if (_workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.EuCookieLawAccepted, _storeContext.CurrentStore.Id))
        //        //already accepted
        //        return Content("");

        //    //ignore notification?
        //    //right now it's used during logout so popup window is not displayed twice
        //    if (TempData["Grand.IgnoreEuCookieLawWarning"] != null && Convert.ToBoolean(TempData["Grand.IgnoreEuCookieLawWarning"]))
        //        return Content("");

        //    return PartialView();
        //}
       

        #endregion
    }
}
