//using System;
///*using System.Web;*/
////using System.Web.Security;
//using Grand.Core.Domain.Customers;
//using Grand.Services.Customers;
//using Microsoft.AspNetCore.Http;

//namespace Grand.Services.Authentication
//{
//    /// <summary>
//    /// Authentication service
//    /// </summary>
//    public partial class FormsAuthenticationService : IAuthenticationService
//    {
//        #region Fields

//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly ICustomerService _customerService;
//        private readonly CustomerSettings _customerSettings;
//        private readonly TimeSpan _expirationTimeSpan;

//        private Customer _cachedCustomer;

//        #endregion

//        #region Ctor

//        /// <summary>
//        /// Ctor
//        /// </summary>
//        /// <param name="httpContextAccessor">HTTP context</param>
//        /// <param name="customerService">Customer service</param>
//        /// <param name="customerSettings">Customer settings</param>
//        public FormsAuthenticationService(IHttpContextAccessor httpContextAccessor,
//            ICustomerService customerService, CustomerSettings customerSettings)
//        {
//            this._httpContextAccessor = httpContextAccessor;
//            this._customerService = customerService;
//            this._customerSettings = customerSettings;
//            //this._expirationTimeSpan = FormsAuthentication.Timeout;
//        }

//        #endregion

//        #region Utilities

//        /// <summary>
//        /// Get authenticated customer
//        /// </summary>
//        /// <param name="ticket">Ticket</param>
//        /// <returns>Customer</returns>
//        //protected virtual Customer GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
//        //{
//        //    if (ticket == null)
//        //        throw new ArgumentNullException("ticket");

//        //    var usernameOrEmail = ticket.UserData;

//        //    if (String.IsNullOrWhiteSpace(usernameOrEmail))
//        //        return null;
//        //    var customer = _customerSettings.UsernamesEnabled
//        //        ? _customerService.GetCustomerByUsername(usernameOrEmail)
//        //        : _customerService.GetCustomerByEmail(usernameOrEmail);
//        //    return customer;
//        //}

//        #endregion

//        #region Methods

//        /// <summary>
//        /// Sign in
//        /// </summary>
//        /// <param name="customer">Customer</param>
//        /// <param name="createPersistentCookie">A value indicating whether to create a persistent cookie</param>
//        public virtual void SignIn(Customer customer, bool createPersistentCookie)
//        {
//            var now = DateTime.UtcNow.ToLocalTime();

//            var ticket = new FormsAuthenticationTicket(
//                1 /*version*/,
//                _customerSettings.UsernamesEnabled ? customer.Username : customer.Email,
//                now,
//                now.Add(_expirationTimeSpan),
//                createPersistentCookie,
//                _customerSettings.UsernamesEnabled ? customer.Username : customer.Email,
//                FormsAuthentication.FormsCookiePath);

//            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

//            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
//            cookie.HttpOnly = true;
//            if (ticket.IsPersistent)
//            {
//                cookie.Expires = ticket.Expiration;
//            }
//            cookie.Secure = FormsAuthentication.RequireSSL;
//            cookie.Path = FormsAuthentication.FormsCookiePath;
//            if (FormsAuthentication.CookieDomain != null)
//            {
//                cookie.Domain = FormsAuthentication.CookieDomain;
//            }

//            _httpContextAccessor.Response.Cookies.Add(cookie);
//            _cachedCustomer = customer;
//        }

//        /// <summary>
//        /// Sign out
//        /// </summary>
//        public virtual void SignOut()
//        {
//            //_cachedCustomer = null;
//            //FormsAuthentication.SignOut();
//        }

//        /// <summary>
//        /// Get authenticated customer
//        /// </summary>
//        /// <returns>Customer</returns>
//        public virtual Customer GetAuthenticatedCustomer()
//        {
//            if (_cachedCustomer != null)
//                return _cachedCustomer;

//            //if (_httpContextAccessor == null ||
//            //    _httpContextAccessor.Request == null ||
//            //    !_httpContextAccessor.Request.IsAuthenticated ||
//            //    !(_httpContextAccessor.User.Identity is FormsIdentity))
//            //{
//            //    return null;
//            //}

//            //var formsIdentity = (FormsIdentity)_httpContextAccessor.User.Identity;
//            //var customer = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
//            //if (customer != null && customer.Active && !customer.Deleted && customer.IsRegistered())
//            //    _cachedCustomer = customer;
//            return _cachedCustomer;
//        }

//        #endregion

//    }
//}