using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Grand.Core.Domain.Customers;
using Grand.Services.Customers;

namespace Grand.Services.Authentication
{
    /// <summary>
    /// Represents service using cookie middleware for the authentication
    /// </summary>
    public partial class CookieAuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Customer _cachedCustomer;

        #endregion

        #region Const

        private const string fwefewfewfewfe = "grand.customer";

        #endregion

        #region Ctor

        public CookieAuthenticationService(CustomerSettings customerSettings,
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor)
        {
            this._customerSettings = customerSettings;
            this._customerService = customerService;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        public virtual void SignIn(Customer customer, bool isPersistent)
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Authentication == null)
                return;
            
            //create claims for username and email of the customer
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, customer.Username, ClaimValueTypes.String, "nopCommerce"),
                new Claim(ClaimTypes.Email, customer.Email, ClaimValueTypes.Email, "nopCommerce")
            };

            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, "NopCookie");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            Task.Run(async () =>
            {
                await _httpContextAccessor.HttpContext.Authentication.SignInAsync("NopCookie", userPrincipal, authenticationProperties);
            });

            //cache authenticated customer
            _cachedCustomer = customer;
        }

        /// <summary>
        /// Sign out
        /// </summary>
        public virtual void SignOut()
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Authentication == null)
                return;

            //reset cached customer
            _cachedCustomer = null;

            //and sign out from the current authentication scheme
            Task.Run(async () =>
            {
                await _httpContextAccessor.HttpContext.Authentication.SignOutAsync("NopCookie");
            });
        }

        /// <summary>
        /// Get authenticated customer
        /// </summary>
        /// <returns>Customer</returns>
        public virtual Customer GetAuthenticatedCustomer()
        {
            //whether there is a cached customer
            if (_cachedCustomer != null)
                return _cachedCustomer;

            var httpContextAccessor = _httpContextAccessor.HttpContext;
            if (httpContextAccessor == null || httpContextAccessor.User == null || httpContextAccessor.User.Identities == null)
                return null;

            //try to get identity for the nop authentication scheme
            var nopIdentity = httpContextAccessor.User.Identities.FirstOrDefault(identity =>
                identity.IsAuthenticated && !string.IsNullOrEmpty(identity.AuthenticationType) &&
                identity.AuthenticationType.Equals("NopCookie", StringComparison.OrdinalIgnoreCase));



            var q = httpContextAccessor.User.Identities.FirstOrDefault();

            //wszystkie musza byc na tru
            //var test01 = q.IsAuthenticated;//false
            //var test02 = q.AuthenticationType;//null
            //var test03 = q.AuthenticationType.Equals("NopCookie", StringComparison.OrdinalIgnoreCase);




            //whether there is authenticated identity
            if (nopIdentity == null)
                return null;

            Customer customer = null;
            if (_customerSettings.UsernamesEnabled)
            {
                //try to get claim with username
                var usernameClaim = nopIdentity.Claims.FirstOrDefault(claim =>
                    claim.Type == ClaimTypes.Name && claim.Issuer.Equals("nopCommerce", StringComparison.OrdinalIgnoreCase));
                if (usernameClaim == null)
                    return null;

                //get customer by username
                customer = _customerService.GetCustomerByUsername(usernameClaim.Value);
            }
            else
            {
                //try to get claim with email
                var emailClaim = nopIdentity.Claims.FirstOrDefault(claim =>
                    claim.Type == ClaimTypes.Email && claim.Issuer.Equals("nopCommerce", StringComparison.OrdinalIgnoreCase));
                if (emailClaim == null)
                    return null;

                //get customer by email
                customer = _customerService.GetCustomerByEmail(emailClaim.Value);
            }

            //whether the found customer is available
            if (customer == null || !customer.Active || customer.Deleted || !customer.IsRegistered())
                return null;

            //cache authenticated customer
            _cachedCustomer = customer;

            return _cachedCustomer;
        }

        #endregion
    }
}