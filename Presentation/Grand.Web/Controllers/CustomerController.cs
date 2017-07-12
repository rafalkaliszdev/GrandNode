﻿using System;
using System.Linq;
//using System.Web;
using Microsoft.AspNetCore.Mvc;
using Grand.Core;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Forums;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Messages;
using Grand.Core.Domain.Tax;
using Grand.Services.Authentication;
using Grand.Services.Authentication.External;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Helpers;
using Grand.Services.Localization;
using Grand.Services.Logging;
using Grand.Services.Media;
using Grand.Services.Messages;
using Grand.Services.Orders;
using Grand.Services.Tax;
using Grand.Web.Extensions;
using Grand.Web.Framework;
using Grand.Web.Framework.Controllers;
using Grand.Web.Framework.Security;
using Grand.Web.Framework.Security.Captcha;
//using Grand.Web.Framework.Security.Honeypot;
using Grand.Web.Models.Customer;
using Grand.Services.Events;
using Grand.Core.Domain;
using Grand.Web.Services;
using Grand.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Grand.Web.Controllers
{
    public partial class CustomerController : BasePublicController
    {
        #region Fields
        private readonly ICustomerWebService _customerWebService;
        private readonly IAuthenticationService _authenticationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ITaxService _taxService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICountryService _countryService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressWebService _addressWebService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Ctor

        public CustomerController(
            ICustomerWebService customerWebService,
            IAuthenticationService authenticationService,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService customerService,
            ICustomerAttributeParser customerAttributeParser,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ITaxService taxService,
            CustomerSettings customerSettings,
            ICountryService countryService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IShoppingCartService shoppingCartService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IAddressWebService addressWebService,
            IEventPublisher eventPublisher,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings
            )
        {
            this._customerWebService = customerWebService;
            this._authenticationService = authenticationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._customerAttributeParser = customerAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._taxService = taxService;
            this._customerSettings = customerSettings;
            this._countryService = countryService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._shoppingCartService = shoppingCartService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._addressWebService = addressWebService;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities


        /// <summary>
        /// Prepare custom customer attribute models
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="overrideAttributesXml">When specified we do not use attributes of a customer</param>
        /// <returns>A list of customer attribute models</returns>


        #endregion

        #region Login / logout

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        //[StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var model = _customerWebService.PrepareLogin(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        //available even when a store is closed
        //[StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        ////[CaptchaValidator]
        public virtual IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            //new class implementing IAuthservice
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", "", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View(model);
        }

        //available even when a store is closed
        //[StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual IActionResult Logout()
        {
            //external authentication
            //ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //logout impersonated customer
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                    SystemCustomerAttributeNames.ImpersonatedCustomerId, null);

                //redirect back to customer details page (admin area)
                return this.RedirectToAction("Edit", "Customer", new { id = _workContext.CurrentCustomer.Id, area = "Admin" });

            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", "", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));
            //standard logout 
            _authenticationService.SignOut();

            //EU Cookie
            if (EngineContextExperimental.Current.Resolve<StoreInformationSettings>().DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["Grand.IgnoreEuCookieLawWarning"] = true;
            }
            return RedirectToRoute("HomePage");
        }

        #endregion

        #region Password recovery

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual IActionResult PasswordRecovery()
        //{
        //    var model = _customerWebService.PreparePasswordRecovery();
        //    return View(model);
        //}

        //[HttpPost, ActionName("PasswordRecovery")]
        ////[PublicAntiForgery]
        //[FormValueRequired("send-email")]
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual IActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var customer = _customerService.GetCustomerByEmail(model.Email);
        //        if (customer != null && customer.Active && !customer.Deleted)
        //        {
        //            _customerWebService.PasswordRecoverySend(model, customer);
        //            model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
        //        }
        //        else
        //        {
        //            model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
        //        }

        //        return View(model);
        //    }

        //    //If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual IActionResult PasswordRecoveryConfirm(string token, string email)
        //{
        //    var customer = _customerService.GetCustomerByEmail(email);
        //    if (customer == null)
        //        return RedirectToRoute("HomePage");

        //    var model = _customerWebService.PreparePasswordRecovery();

        //    return View(model);
        //}

        //[HttpPost, ActionName("PasswordRecoveryConfirm")]
        ////[PublicAntiForgery]
        //[FormValueRequired("set-password")]
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual IActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        //{
        //    var customer = _customerService.GetCustomerByEmail(email);
        //    if (customer == null)
        //        return RedirectToRoute("HomePage");

        //    //validate token
        //    if (!customer.IsPasswordRecoveryTokenValid(token))
        //    {
        //        model.DisablePasswordChanging = true;
        //        model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
        //    }

        //    //validate token expiration date
        //    if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
        //    {
        //        model.DisablePasswordChanging = true;
        //        model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
        //        return View(model);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(email,
        //            false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
        //        if (response.Success)
        //        {
        //            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, "");

        //            model.DisablePasswordChanging = true;
        //            model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
        //        }
        //        else
        //        {
        //            model.Result = response.Errors.FirstOrDefault();
        //        }

        //        return View(model);
        //    }

        //    //If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        #endregion

        #region Register

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual IActionResult Register()
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model = _customerWebService.PrepareRegisterModel(model, false);
            //enable newsletter by default
            model.Newsletter = _customerSettings.NewsletterTickedByDefault;

            return View(model);
        }

        [HttpPost]
        ////[CaptchaValidator]
        //[HoneypotValidator]
        ////[PublicAntiForgery]
        //[ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual IActionResult Register(RegisterModel model, string returnUrl, bool captchaValid, IFormCollection form)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;

            //custom customer attributes
            var customerAttributesXml = _customerWebService.ParseCustomAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer, model.Email,
                    _customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password,
                    _customerSettings.DefaultPasswordFormat, _storeContext.CurrentStore.Id, isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

                        string vatName;
                        string vatAddress;
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                        _genericAttributeService.SaveAttribute(customer,
                            SystemCustomerAttributeNames.VatNumberStatusId,
                            (int)vatNumberStatus);

                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    CustomerId = customer.Id,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    _customerWebService.TryAssociateAccountWithExternalAccount(customer);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                        LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                        Email = customer.Email,
                        Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                        CountryId = !String.IsNullOrEmpty(customer.GetAttribute<string>(SystemCustomerAttributeNames.CountryId)) ?
                            customer.GetAttribute<string>(SystemCustomerAttributeNames.CountryId) : "",
                        StateProvinceId = !String.IsNullOrEmpty(customer.GetAttribute<string>(SystemCustomerAttributeNames.StateProvinceId)) ?
                            customer.GetAttribute<string>(SystemCustomerAttributeNames.StateProvinceId) : "",
                        City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                        Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                        Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                        ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                        PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                        FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                        CreatedOnUtc = customer.CreatedOnUtc,
                    };
                    var addressService = EngineContextExperimental.Current.Resolve<IAddressService>();
                    if (addressService.IsAddressValid(defaultAddress))
                    {
                        //set default address
                        defaultAddress.CustomerId = customer.Id;
                        customer.Addresses.Add(defaultAddress);
                        _customerService.InsertAddress(defaultAddress);
                        customer.BillingAddress = defaultAddress;
                        _customerService.UpdateBillingAddress(defaultAddress);
                        customer.ShippingAddress = defaultAddress;
                        _customerService.UpdateShippingAddress(defaultAddress);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    //New customer has a free shipping for the first order
                    if (_customerSettings.RegistrationFreeShipping)
                        _customerService.UpdateFreeShipping(customer.Id, true);

                    EngineContextExperimental.Current.Resolve<ICustomerActionEventService>().Registration(customer);

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + System.Net.WebUtility.UrlEncode(returnUrl), null);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _customerWebService.PrepareRegisterModel(model, true, customerAttributesXml);
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual IActionResult RegisterResult(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel
            {
                Result = resultText
            };
            return View(model);
        }

        //[HttpPost]
        ////[PublicAntiForgery]
        //[ValidateInput(false)]
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual IActionResult CheckUsernameAvailability(string username)
        //{
        //    var usernameAvailable = false;
        //    var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

        //    if (_customerSettings.UsernamesEnabled && !String.IsNullOrWhiteSpace(username))
        //    {
        //        if (_workContext.CurrentCustomer != null &&
        //            _workContext.CurrentCustomer.Username != null &&
        //            _workContext.CurrentCustomer.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
        //        {
        //            statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
        //        }
        //        else
        //        {
        //            var customer = _customerService.GetCustomerByUsername(username);
        //            if (customer == null)
        //            {
        //                statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
        //                usernameAvailable = true;
        //            }
        //        }
        //    }

        //    return Json(new { Available = usernameAvailable, Text = statusText });
        //}

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        ////available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        //public virtual IActionResult AccountActivation(string token, string email)
        //{
        //    var customer = _customerService.GetCustomerByEmail(email);
        //    if (customer == null)
        //        return RedirectToRoute("HomePage");

        //    var cToken = customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken);
        //    if (String.IsNullOrEmpty(cToken))
        //        return RedirectToRoute("HomePage");

        //    if (!cToken.Equals(token, StringComparison.OrdinalIgnoreCase))
        //        return RedirectToRoute("HomePage");

        //    //activate user account
        //    customer.Active = true;
        //    _customerService.UpdateActive(customer);
        //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, "");

        //    //send welcome message
        //    _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

        //    var model = new AccountActivationModel();
        //    model.Result = _localizationService.GetResource("Account.AccountActivation.Activated");
        //    return View(model);
        //}

        //#endregion

        //#region My account / Info

        //[ChildActionOnly]
        //public virtual IActionResult CustomerNavigation(int selectedTabId = 0)
        //{
        //    var model = _customerWebService.PrepareNavigation(selectedTabId);
        //    return PartialView(model);
        //}

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult Info()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;

        //    var model = new CustomerInfoModel();
        //    model = _customerWebService.PrepareInfoModel(model, customer, false);

        //    return View(model);
        //}

        //[HttpPost]
        ////[PublicAntiForgery]
        //[ValidateInput(false)]
        //public virtual IActionResult Info(CustomerInfoModel model, IFormCollection form)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;

        //    //custom customer attributes
        //    var customerAttributesXml = _customerWebService.ParseCustomAttributes(form);
        //    var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
        //    foreach (var error in customerAttributeWarnings)
        //    {
        //        ModelState.AddModelError("", error);
        //    }

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //username 
        //            if (_customerSettings.UsernamesEnabled && this._customerSettings.AllowUsersToChangeUsernames)
        //            {
        //                if (!customer.Username.Equals(model.Username.Trim(), StringComparison.OrdinalIgnoreCase))
        //                {
        //                    //change username
        //                    _customerRegistrationService.SetUsername(customer, model.Username.Trim());
        //                    //re-authenticate
        //                    if (_workContext.OriginalCustomerIfImpersonated == null)
        //                        _authenticationService.SignIn(customer, true);
        //                }
        //            }
        //            //email
        //            if (!customer.Email.Equals(model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        //            {
        //                //change email
        //                _customerRegistrationService.SetEmail(customer, model.Email.Trim());
        //                //re-authenticate (if usernames are disabled)
        //                //do not authenticate users in impersonation mode
        //                if (_workContext.OriginalCustomerIfImpersonated == null)
        //                {
        //                    //re-authenticate (if usernames are disabled)
        //                    if (!_customerSettings.UsernamesEnabled)
        //                        _authenticationService.SignIn(customer, true);
        //                }
        //            }

        //            //properties
        //            if (_dateTimeSettings.AllowCustomersToSetTimeZone)
        //            {
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
        //            }
        //            //VAT number
        //            if (_taxSettings.EuVatEnabled)
        //            {
        //                var prevVatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

        //                if (prevVatNumber != model.VatNumber)
        //                {
        //                    string vatName;
        //                    string vatAddress;
        //                    var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
        //                    _genericAttributeService.SaveAttribute(customer,
        //                            SystemCustomerAttributeNames.VatNumberStatusId,
        //                            (int)vatNumberStatus);

        //                    //send VAT number admin notification
        //                    if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
        //                        _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
        //                }
        //            }

        //            //form fields
        //            if (_customerSettings.GenderEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
        //            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
        //            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
        //            if (_customerSettings.DateOfBirthEnabled)
        //            {
        //                DateTime? dateOfBirth = model.ParseDateOfBirth();
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
        //            }
        //            if (_customerSettings.CompanyEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
        //            if (_customerSettings.StreetAddressEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
        //            if (_customerSettings.StreetAddress2Enabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
        //            if (_customerSettings.ZipPostalCodeEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
        //            if (_customerSettings.CityEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
        //            if (_customerSettings.CountryEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
        //            if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
        //            if (_customerSettings.PhoneEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
        //            if (_customerSettings.FaxEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

        //            //newsletter
        //            if (_customerSettings.NewsletterEnabled)
        //            {
        //                //save newsletter value
        //                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
        //                if (newsletter == null)
        //                    newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByCustomerId(customer.Id);

        //                if (newsletter != null)
        //                {
        //                    if (model.Newsletter)
        //                    {
        //                        newsletter.Active = true;
        //                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
        //                    }
        //                    else
        //                    {
        //                        newsletter.Active = false;
        //                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
        //                    }
        //                }
        //                else
        //                {
        //                    if (model.Newsletter)
        //                    {
        //                        _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
        //                        {
        //                            NewsLetterSubscriptionGuid = Guid.NewGuid(),
        //                            Email = customer.Email,
        //                            CustomerId = customer.Id,
        //                            Active = true,
        //                            StoreId = _storeContext.CurrentStore.Id,
        //                            CreatedOnUtc = DateTime.UtcNow
        //                        });
        //                    }
        //                }
        //            }
        //            var forumSettings = EngineContextExperimental.Current.Resolve<ForumSettings>();
        //            if (forumSettings.ForumsEnabled && forumSettings.SignaturesEnabled)
        //                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Signature, model.Signature);

        //            //save customer attributes
        //            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

        //            return RedirectToRoute("CustomerInfo");
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        ModelState.AddModelError("", exc.Message);
        //    }

        //    //If we got this far, something failed, redisplay form
        //    model = _customerWebService.PrepareInfoModel(model, customer, true, customerAttributesXml);
        //    return View(model);
        //}

        //[HttpPost]
        ////[PublicAntiForgery]
        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult RemoveExternalAssociation(string id)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var openAuthenticationService = EngineContextExperimental.Current.Resolve<IOpenAuthenticationService>();
        //    //ensure it's our record
        //    var ear = openAuthenticationService.GetExternalIdentifiersFor(_workContext.CurrentCustomer)
        //        .FirstOrDefault(x => x.Id == id);

        //    if (ear == null)
        //    {
        //        return Json(new
        //        {
        //            redirect = Url.Action("Info"),
        //        });
        //    }
        //    openAuthenticationService.DeletExternalAuthenticationRecord(ear);

        //    return Json(new
        //    {
        //        redirect = Url.Action("Info"),
        //    });
        //}

        //#endregion

        //#region My account / Addresses

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult Addresses()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var model = _customerWebService.PrepareAddressList(_workContext.CurrentCustomer);
        //    return View(model);
        //}

        //[HttpPost]
        ////[PublicAntiForgery]
        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult AddressDelete(string addressId)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;

        //    //find address (ensure that it belongs to the current customer)
        //    var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
        //    if (address != null)
        //    {
        //        customer.RemoveAddress(address);
        //        address.CustomerId = customer.Id;
        //        _customerService.DeleteAddress(address);
        //    }

        //    return Json(new
        //    {
        //        redirect = Url.RouteUrl("CustomerAddresses"),
        //    });

        //}

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult AddressAdd()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var model = new CustomerAddressEditModel();
        //    _addressWebService.PrepareModel(model: model.Address,
        //        address: null,
        //        excludeProperties: false,
        //        loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

        //    return View(model);
        //}

        //[HttpPost]
        ////[PublicAntiForgery]
        //[ValidateInput(false)]
        //public virtual IActionResult AddressAdd(CustomerAddressEditModel model, IFormCollection form)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;

        //    //custom address attributes
        //    var customAttributes = _addressWebService.ParseCustomAddressAttributes(form);
        //    var customAttributeWarnings = _addressWebService.GetAttributeWarnings(customAttributes);
        //    foreach (var error in customAttributeWarnings)
        //    {
        //        ModelState.AddModelError("", error);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var address = model.Address.ToEntity();
        //        address.CustomAttributes = customAttributes;
        //        address.CreatedOnUtc = DateTime.UtcNow;
        //        customer.Addresses.Add(address);
        //        address.CustomerId = customer.Id;

        //        _customerService.InsertAddress(address);

        //        return RedirectToRoute("CustomerAddresses");
        //    }

        //    //If we got this far, something failed, redisplay form
        //    _addressWebService.PrepareModel(model: model.Address,
        //        address: null,
        //        excludeProperties: true,
        //        loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

        //    return View(model);
        //}

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult AddressEdit(string addressId)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;
        //    //find address (ensure that it belongs to the current customer)
        //    var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
        //    if (address == null)
        //        //address is not found
        //        return RedirectToRoute("CustomerAddresses");

        //    var model = new CustomerAddressEditModel();
        //    _addressWebService.PrepareModel(model: model.Address,
        //        address: address,
        //        excludeProperties: false,
        //        loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

        //    return View(model);
        //}

        //[HttpPost]
        ////[PublicAntiForgery]
        //[ValidateInput(false)]
        //public virtual IActionResult AddressEdit(CustomerAddressEditModel model, string addressId, IFormCollection form)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;
        //    //find address (ensure that it belongs to the current customer)
        //    var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
        //    if (address == null)
        //        //address is not found
        //        return RedirectToRoute("CustomerAddresses");

        //    //custom address attributes
        //    var customAttributes = _addressWebService.ParseCustomAddressAttributes(form);
        //    var customAttributeWarnings = _addressWebService.GetAttributeWarnings(customAttributes);
        //    foreach (var error in customAttributeWarnings)
        //    {
        //        ModelState.AddModelError("", error);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        address = model.Address.ToEntity(address);
        //        address.CustomAttributes = customAttributes;
        //        address.CustomerId = customer.Id;
        //        _customerService.UpdateAddress(address);

        //        return RedirectToRoute("CustomerAddresses");
        //    }

        //    //If we got this far, something failed, redisplay form
        //    _addressWebService.PrepareModel(model: model.Address,
        //        address: address,
        //        excludeProperties: true,
        //        loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));
        //    return View(model);
        //}

        //#endregion

        //#region My account / Downloadable products

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult DownloadableProducts()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    if (_customerSettings.HideDownloadableProductsTab)
        //        return RedirectToRoute("CustomerInfo");

        //    var model = _customerWebService.PrepareDownloadableProducts(_workContext.CurrentCustomer.Id);
        //    return View(model);
        //}

        //public virtual IActionResult UserAgreement(Guid orderItemId)
        //{
        //    var model = _customerWebService.PrepareUserAgreement(orderItemId);
        //    if (model == null)
        //        return RedirectToRoute("HomePage");

        //    return View(model);
        //}

        //#endregion

        //#region My account / Change password

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult ChangePassword()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var model = new ChangePasswordModel();

        //    //display the cause of the change password 
        //    if (_workContext.CurrentCustomer.PasswordIsExpired())
        //        ModelState.AddModelError(string.Empty, _localizationService.GetResource("Account.ChangePassword.PasswordIsExpired"));

        //    return View(model);
        //}

        //[HttpPost]
        ////[PublicAntiForgery]
        //public virtual IActionResult ChangePassword(ChangePasswordModel model)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    var customer = _workContext.CurrentCustomer;

        //    if (ModelState.IsValid)
        //    {
        //        var changePasswordRequest = new ChangePasswordRequest(customer.Email,
        //            true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
        //        var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
        //        if (changePasswordResult.Success)
        //        {
        //            model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
        //            return View(model);
        //        }

        //        //errors
        //        foreach (var error in changePasswordResult.Errors)
        //            ModelState.AddModelError("", error);
        //    }


        //    //If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //#endregion

        //#region My account / Avatar

        ////[GrandHttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult Avatar()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        return RedirectToRoute("CustomerInfo");

        //    var model = _customerWebService.PrepareAvatar(_workContext.CurrentCustomer);

        //    return View(model);
        //}

        //[HttpPost, ActionName("Avatar")]
        ////[PublicAntiForgery]
        //[FormValueRequired("upload-avatar")]
        //public virtual IActionResult UploadAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        return RedirectToRoute("CustomerInfo");

        //    var customer = _workContext.CurrentCustomer;

        //    var pictureService = EngineContextExperimental.Current.Resolve<IPictureService>();
        //    var mediaSettings = EngineContextExperimental.Current.Resolve<MediaSettings>();

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var customerAvatar = pictureService.GetPictureById(customer.GetAttribute<string>(SystemCustomerAttributeNames.AvatarPictureId));
        //            if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
        //            {
        //                int avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
        //                if (uploadedFile.ContentLength > avatarMaxSize)
        //                    throw new GrandException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

        //                byte[] customerPictureBinary = uploadedFile.GetPictureBits();
        //                if (customerAvatar != null)
        //                    customerAvatar = pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
        //                else
        //                    customerAvatar = pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, null);
        //            }

        //            string customerAvatarId = "";
        //            if (customerAvatar != null)
        //                customerAvatarId = customerAvatar.Id;

        //            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

        //            model.AvatarUrl = pictureService.GetPictureUrl(
        //                customer.GetAttribute<string>(SystemCustomerAttributeNames.AvatarPictureId),
        //                false,
        //                mediaSettings.AvatarPictureSize,
        //                false);
        //            return View(model);
        //        }
        //        catch (Exception exc)
        //        {
        //            ModelState.AddModelError("", exc.Message);
        //        }
        //    }


        //    //If we got this far, something failed, redisplay form
        //    model.AvatarUrl = pictureService.GetPictureUrl(
        //        customer.GetAttribute<string>(SystemCustomerAttributeNames.AvatarPictureId),
        //        false,
        //        mediaSettings.AvatarPictureSize,
        //        false);
        //    return View(model);
        //}

        //[HttpPost, ActionName("Avatar")]
        ////[PublicAntiForgery]
        //[FormValueRequired("remove-avatar")]
        //public virtual IActionResult RemoveAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return new UnauthorizedResult();

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        return RedirectToRoute("CustomerInfo");

        //    var customer = _workContext.CurrentCustomer;
        //    var pictureService = EngineContextExperimental.Current.Resolve<IPictureService>();

        //    var customerAvatar = pictureService.GetPictureById(customer.GetAttribute<string>(SystemCustomerAttributeNames.AvatarPictureId));
        //    if (customerAvatar != null)
        //        pictureService.DeletePicture(customerAvatar);
        //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, 0);

        //    return RedirectToRoute("CustomerAvatar");
        //}

        #endregion
    }
}
