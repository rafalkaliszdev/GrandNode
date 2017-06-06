using System;
using System.Collections.Generic;
//using System.Web.Mvc;
//using System.Web.Routing;
using Grand.Web.Framework.Localization;
//using Grand.Web.Framework.Mvc.Routes;
using Microsoft.AspNetCore.Builder;



//workaround
using Grand.Core.Configuration.Routes;


namespace Grand.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public List<NameTemplateDefaults> CollectRoutes(List<NameTemplateDefaults> routes)
        {
            //We reordered our routes so the most used ones are on top. It can improve performance.
            //app.UseMvc(routes =>
            //{










            /*

            routes.MapRoute(
                name: "default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "Main", action = "Index" });

            routes.MapRoute(
                name: "default",
                template: "{controller=Main}/{action=Index}/{id?}");

            */








            //2017_06_05
            //it is strange, this route had not been existing before
            //but now apparently it is required 
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutOpcSaveBilling",
            //  "checkout/OpcSaveBilling",
            //  new { controller = "Checkout", action = "OpcSaveBilling" }));



            ////test
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("WhyItDoesntWork",
            //  "checkout/WhyItDoesntWork",
            //  new { controller = "Checkout", action = "WhyItDoesntWork" }));





            #region only-test-routes (they are duplicated, so dont worry about removing them later)


            ////home page
            //routes.Add(new NameTemplateDefaults(
            //    "HomePage",
            //    "",
            //    new { controller = "Home", action = "Index" }));

            ////add to cart
            //routes.Add(new NameTemplateDefaults(
            //    "AddProductToCart-Catalog",
            //    "addproducttocart/catalog/{productId}/{shoppingCartTypeId}/{quantity}",
            //    new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" }));

            ////shopping cart
            //routes.Add(new NameTemplateDefaults(
            //    "ShoppingCart",
            //    "cart/",
            //    new { controller = "ShoppingCart", action = "Cart" }));




















            //2017_05_22



            //below routes are good and working, but i dont need them now, and they may obscure what im doing now
            //return routes;
            #endregion








            //home page
            routes.Add(new NameTemplateDefaults(
                "HomePage",
                "",
                new { controller = "Home", action = "Index" }));

            //widgets
            //we have this route for performance optimization because named routes are MUCH faster than usual Html.Action(...)
            //and this route is highly used
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("WidgetsByZone",
                            "widgetsbyzone/",
                            new { controller = "Widget", action = "WidgetsByZone" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //login
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Login",
                            "login/",
                            new { controller = "Customer", action = "Login" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //register
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Register",
                            "register/",
                            new { controller = "Customer", action = "Register" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //logout
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Logout",
                            "logout/",
                            new { controller = "Customer", action = "Logout" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //shopping cart
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ShoppingCart",
                            "cart/",
                            new { controller = "ShoppingCart", action = "Cart" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //estimate shipping
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("EstimateShipping",
                            "cart/estimateshipping",
                            new { controller = "ShoppingCart", action = "GetEstimateShipping" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            ////wishlist
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Wishlist",
            //                "wishlist/{customerGuid}",
            //                new { controller = "ShoppingCart", action = "Wishlist", customerGuid = UrlParameter.Optional }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //customer account links
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerInfo",
                            "customer/info",
                            new { controller = "Customer", action = "Info" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerAddresses",
                            "customer/addresses",
                            new { controller = "Customer", action = "Addresses" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerOrders",
                            "order/history",
                            new { controller = "Order", action = "CustomerOrders" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //contact us
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ContactUs",
                            "contactus",
                            new { controller = "Common", action = "ContactUs" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //sitemap
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Sitemap",
                            "sitemap",
                            new { controller = "Common", action = "Sitemap" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("sitemap-indexed.xml",
            //                "sitemap-{Id}.xml",
            //                new { controller = "Common", action = "SitemapXml" }));
            //                new { Id = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //interactive form
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PopupInteractiveForm",
                            "popupinteractiveform",
                            new { controller = "Common", action = "PopupInteractiveForm" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //product search
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ProductSearch",
                            "search/",
                            new { controller = "Catalog", action = "Search" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ProductSearchAutoComplete",
                            "catalog/searchtermautocomplete",
                            new { controller = "Catalog", action = "SearchTermAutoComplete" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            ////change currency (AJAX link)
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ChangeCurrency",
            //                "changecurrency/{customercurrency}",
            //                new { controller = "Common", action = "SetCurrency" }));
            //                new { customercurrency = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////change language (AJAX link)
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ChangeLanguage",
            //                "changelanguage/{langid}",
            //                new { controller = "Common", action = "SetLanguage" }));
            //                new { langid = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////change tax (AJAX link)
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ChangeTaxType",
            //                "changetaxtype/{customertaxtype}",
            //                new { controller = "Common", action = "SetTaxType" }));
            //                new { customertaxtype = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //recently viewed products
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("RecentlyViewedProducts",
                            "recentlyviewedproducts/",
                            new { controller = "Product", action = "RecentlyViewedProducts" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //new products
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("NewProducts",
                            "newproducts/",
                            new { controller = "Product", action = "NewProducts" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //blog
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Blog",
                            "blog",
                            new { controller = "Blog", action = "List" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //news
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("NewsArchive",
                            "news",
                            new { controller = "News", action = "List" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //forum
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Boards",
                            "boards",
                            new { controller = "Boards", action = "Index" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //compare products
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CompareProducts",
                            "compareproducts/",
                            new { controller = "Product", action = "CompareProducts" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //product tags
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ProductTagsAll",
                            "producttag/all/",
                            new { controller = "Catalog", action = "ProductTagsAll" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //manufacturers
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ManufacturerList",
                            "manufacturer/all/",
                            new { controller = "Catalog", action = "ManufacturerAll" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //vendors
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("VendorList",
                            "vendor/all/",
                            new { controller = "Catalog", action = "VendorAll" }));
            /*new[] { "Grand.Web.Controllers" }));*/







            //add product to cart (without any attributes and options). used on catalog pages.
            routes.Add(new NameTemplateDefaults(
                "AddProductToCart-Catalog",
                "addproducttocart/catalog/{productId}/{shoppingCartTypeId}/{quantity}",
                new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" }));
            //new { productId = @"\w+", shoppingCartTypeId = @"\d+", quantity = @"\d+" }));
            /*new[] { "Grand.Web.Controllers" }));*/




            ////add product to cart (with attributes and options). used on the product details pages.
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("AddProductToCart-Details",
            //                "addproducttocart/details/{productId}/{shoppingCartTypeId}",
            //                new { controller = "ShoppingCart", action = "AddProductToCart_Details" }));
            //new { productId = @"\w+", shoppingCartTypeId = @"\d+" }));
            ///*new[] { "Grand.Web.Controllers" }));*/

            ////product tags
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ProductsByTag",
            //                "producttag/{productTagId}/{SeName}",
            //                new { controller = "Catalog", action = "ProductsByTag", SeName = UrlParameter.Optional }));
            //                new { productTagId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////comparing products
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("AddProductToCompare",
            //                "compareproducts/add/{productId}",
            //                new { controller = "Product", action = "AddProductToCompareList" }));
            //                new { productId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////product email a friend
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ProductEmailAFriend",
            //                "productemailafriend/{productId}",
            //                new { controller = "Product", action = "ProductEmailAFriend" }));
            //                new { productId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////product ask question
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("AskQuestion",
            //                "askquestion/{productId}",
            //                new { controller = "Product", action = "AskQuestion" }));
            //                new { productId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //reviews
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ProductReviews",
                            "productreviews/{productId}",
                            new { controller = "Product", action = "ProductReviews" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            ////back in stock notifications
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("BackInStockSubscribePopup",
            //                "backinstocksubscribe/{productId}",
            //                new { controller = "BackInStockSubscription", action = "SubscribePopup" }));
            //                new { productId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////downloads
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("GetSampleDownload",
            //                "download/sample/{productid}",
            //                new { controller = "Download", action = "Sample" }));
            //                new { productid = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/



            //checkout pages
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Checkout",
                            "checkout/",
                            new { controller = "Checkout", action = "Index" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutOnePage",
                            "onepagecheckout/",
                            new { controller = "Checkout", action = "OnePageCheckout" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutShippingAddress",
                            "checkout/shippingaddress",
                            new { controller = "Checkout", action = "ShippingAddress" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutSelectShippingAddress",
                            "checkout/selectshippingaddress",
                            new { controller = "Checkout", action = "SelectShippingAddress" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutBillingAddress",
                            "checkout/billingaddress",
                            new { controller = "Checkout", action = "BillingAddress" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutSelectBillingAddress",
                            "checkout/selectbillingaddress",
                            new { controller = "Checkout", action = "SelectBillingAddress" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutShippingMethod",
                            "checkout/shippingmethod",
                            new { controller = "Checkout", action = "ShippingMethod" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutPaymentMethod",
                            "checkout/paymentmethod",
                            new { controller = "Checkout", action = "PaymentMethod" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutPaymentInfo",
                            "checkout/paymentinfo",
                            new { controller = "Checkout", action = "PaymentInfo" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutConfirm",
                            "checkout/confirm",
                            new { controller = "Checkout", action = "Confirm" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CheckoutCompleted",
            //                "checkout/completed/{orderId}",
            //                new { controller = "Checkout", action = "Completed", orderId = UrlParameter.Optional }));
            //                new { orderId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //subscribe newsletters
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("SubscribeNewsletter",
                            "subscribenewsletter",
                            new { controller = "Newsletter", action = "SubscribeNewsletter" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //assign newsletters to categories
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("SubscribeNewsletterCategory",
                "newsletter/savecategories",
                new { controller = "Newsletter", action = "SaveCategories" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //email wishlist
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("EmailWishlist",
                            "emailwishlist",
                            new { controller = "ShoppingCart", action = "EmailWishlist" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //login page for checkout as guest
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("LoginCheckoutAsGuest",
                            "login/checkoutasguest",
                            new { controller = "Customer", action = "Login", checkoutAsGuest = true }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //register result page
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/(
                "RegisterResult",
                "registerresult/{resultId}",
                new { controller = "Customer", action = "RegisterResult" }));
            //new { resultId = @"\w+" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //check username availability
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/(
                "CheckUsernameAvailability",
                "customer/checkusernameavailability",
                new { controller = "Customer", action = "CheckUsernameAvailability" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //passwordrecovery
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PasswordRecovery",
                            "passwordrecovery",
                            new { controller = "Customer", action = "PasswordRecovery" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //password recovery confirmation
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PasswordRecoveryConfirm",
                            "passwordrecovery/confirm",
                            new { controller = "Customer", action = "PasswordRecoveryConfirm" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //topics
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicPopup",
                            "t-popup/{SystemName}",
                            new { controller = "Topic", action = "TopicDetailsPopup" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //blog
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("BlogByTag",
                            "blog/tag/{tag}",
                            new { controller = "Blog", action = "BlogByTag" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("BlogByMonth",
                            "blog/month/{month}",
                            new { controller = "Blog", action = "BlogByMonth" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            ////blog RSS
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("BlogRSS",
            //                "blog/rss/{languageId}",
            //                new { controller = "Blog", action = "ListRss" }));
            //                new { languageId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            ////news RSS
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("NewsRSS",
            //                "news/rss/{languageId}",
            //                new { controller = "News", action = "ListRss" }));
            //                new { languageId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //set review helpfulness (AJAX link)
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("SetProductReviewHelpfulness",
                            "setproductreviewhelpfulness",
                            new { controller = "Product", action = "SetProductReviewHelpfulness" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //customer account links
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerReturnRequests",
                            "returnrequest/history",
                            new { controller = "ReturnRequest", action = "CustomerReturnRequests" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerDownloadableProducts",
                            "customer/downloadableproducts",
                            new { controller = "Customer", action = "DownloadableProducts" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerBackInStockSubscriptions",
                            "backinstocksubscriptions/manage",
                            new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerBackInStockSubscriptionsPaged",
            //                "backinstocksubscriptions/manage/{page}",
            //                new { controller = "BackInStockSubscription", action = "CustomerSubscriptions", page = UrlParameter.Optional }));
            //                new { page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerRewardPoints",
                            "rewardpoints/history",
                            new { controller = "Order", action = "CustomerRewardPoints" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerChangePassword",
                            "customer/changepassword",
                            new { controller = "Customer", action = "ChangePassword" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerAvatar",
                            "customer/avatar",
                            new { controller = "Customer", action = "Avatar" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("AccountActivation",
                            "customer/activation",
                            new { controller = "Customer", action = "AccountActivation" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerForumSubscriptions",
                            "boards/forumsubscriptions",
                            new { controller = "Boards", action = "CustomerForumSubscriptions" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerForumSubscriptionsPaged",
            //                "boards/forumsubscriptions/{page}",
            //                new { controller = "Boards", action = "CustomerForumSubscriptions", page = UrlParameter.Optional }));
            //                new { page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerAddressEdit",
            //                "customer/addressedit/{addressId}",
            //                new { controller = "Customer", action = "AddressEdit" }));
            //                new { addressId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerAddressAdd",
                            "customer/addressadd",
                            new { controller = "Customer", action = "AddressAdd" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            ////customer profile page
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerProfile",
            //                "profile/{id}",
            //                new { controller = "Profile", action = "Index" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CustomerProfilePaged",
            //                "profile/{id}/page/{page}",
            //                new { controller = "Profile", action = "Index" }));
            //                new { id = @"\w+", page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            ////orders
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("OrderDetails",
            //                "orderdetails/{orderId}",
            //                new { controller = "Order", action = "Details" }));
            //                new { orderId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ShipmentDetails",
                            "orderdetails/shipment/{shipmentId}",
                            new { controller = "Order", action = "ShipmentDetails" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ReturnRequest",
            //                "returnrequest/{orderId}",
            //                new { controller = "ReturnRequest", action = "ReturnRequest" }));
            //                new { orderId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ReOrder",
            //                "reorder/{orderId}",
            //                new { controller = "Order", action = "ReOrder" }));
            //                new { orderId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("GetOrderPdfInvoice",
                            "orderdetails/pdf/{orderId}",
                            new { controller = "Order", action = "GetPdfInvoice" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PrintOrderDetails",
                            "orderdetails/print/{orderId}",
                            new { controller = "Order", action = "PrintOrderDetails" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("CancelOrder",
                            "orderdetails/cancel/{orderId}",
                            new { controller = "Order", action = "CancelOrder" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            ////order downloads
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("GetDownload",
            //                "download/getdownload/{orderItemId}/{agree}",
            //                new { controller = "Download", action = "GetDownload", agree = UrlParameter.Optional }));
            //                new { orderItemId = new GuidConstraint(false) }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("GetLicense",
            //                "download/getlicense/{orderItemId}/",
            //                new { controller = "Download", action = "GetLicense" }));
            //                new { orderItemId = new GuidConstraint(false) }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("DownloadUserAgreement",
            //                "customer/useragreement/{orderItemId}",
            //                new { controller = "Customer", action = "UserAgreement" }));
            //                new { orderItemId = new GuidConstraint(false) }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("GetOrderNoteFile",
            //                "download/ordernotefile/{ordernoteid}",
            //                new { controller = "Download", action = "GetOrderNoteFile" }));
            //                new { ordernoteid = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //contact vendor
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ContactVendor",
                            "contactvendor/{vendorId}",
                            new { controller = "Common", action = "ContactVendor" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //apply for vendor account
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ApplyVendorAccount",
                            "vendor/apply",
                            new { controller = "Vendor", action = "ApplyVendor" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //poll vote AJAX link
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PollVote",
                            "poll/vote",
                            new { controller = "Poll", action = "Vote" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //comparing products
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("RemoveProductFromCompareList",
                            "compareproducts/remove/{productId}",
                            new { controller = "Product", action = "RemoveProductFromCompareList" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ClearCompareList",
                            "clearcomparelist/",
                            new { controller = "Product", action = "ClearCompareList" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //new RSS
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("NewProductsRSS",
                            "newproducts/rss",
                            new { controller = "Product", action = "NewProductsRss" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //get state list by country ID  (AJAX link)
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("GetStatesByCountryId",
                            "country/getstatesbycountryid/",
                            new { controller = "Country", action = "GetStatesByCountryId" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //EU Cookie law accept button handler (AJAX link)
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("EuCookieLawAccept",
                            "eucookielawaccept",
                            new { controller = "Common", action = "EuCookieLawAccept" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //authenticate topic AJAX link
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicAuthenticate",
                            "topic/authenticate",
                            new { controller = "Topic", action = "Authenticate" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            ////product attributes with "upload file" type
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("UploadFileProductAttribute",
            //                "uploadfileproductattribute/{attributeId}",
            //                new { controller = "ShoppingCart", action = "UploadFileProductAttribute" }));
            //                new { attributeId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            ////checkout attributes with "upload file" type
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("UploadFileCheckoutAttribute",
            //                "uploadfilecheckoutattribute/{attributeId}",
            //                new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" }));
            //                new { attributeId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //forums
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ActiveDiscussions",
                            "boards/activediscussions",
                            new { controller = "Boards", action = "ActiveDiscussions" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ActiveDiscussionsPaged",
            //                "boards/activediscussions/page/{page}",
            //                new { controller = "Boards", action = "ActiveDiscussions", page = UrlParameter.Optional }));
            //                new { page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ActiveDiscussionsRSS",
                            "boards/activediscussionsrss",
                            new { controller = "Boards", action = "ActiveDiscussionsRSS" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PostEdit",
            //                "boards/postedit/{id}",
            //                new { controller = "Boards", action = "PostEdit" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PostDelete",
            //                "boards/postdelete/{id}",
            //                new { controller = "Boards", action = "PostDelete" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PostCreate",
            //                "boards/postcreate/{id}",
            //                new { controller = "Boards", action = "PostCreate" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PostCreateQuote",
            //                "boards/postcreate/{id}/{quote}",
            //                new { controller = "Boards", action = "PostCreate" }));
            //                new { id = @"\w+", quote = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicEdit",
            //                "boards/topicedit/{id}",
            //                new { controller = "Boards", action = "TopicEdit" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicDelete",
            //                "boards/topicdelete/{id}",
            //                new { controller = "Boards", action = "TopicDelete" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicCreate",
            //                "boards/topiccreate/{id}",
            //                new { controller = "Boards", action = "TopicCreate" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicMove",
            //                "boards/topicmove/{id}",
            //                new { controller = "Boards", action = "TopicMove" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicWatch",
            //                "boards/topicwatch/{id}",
            //                new { controller = "Boards", action = "TopicWatch" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicSlug",
            //                "boards/topic/{id}/{slug}",
            //                new { controller = "Boards", action = "Topic", slug = UrlParameter.Optional }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("TopicSlugPaged",
            //                "boards/topic/{id}/{slug}/page/{page}",
            //                new { controller = "Boards", action = "Topic", slug = UrlParameter.Optional, page = UrlParameter.Optional }));
            //                new { id = @"\w+", page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ForumWatch",
            //                "boards/forumwatch/{id}",
            //                new { controller = "Boards", action = "ForumWatch" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ForumRSS",
            //                "boards/forumrss/{id}",
            //                new { controller = "Boards", action = "ForumRSS" }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ForumSlug",
            //                "boards/forum/{id}/{slug}",
            //                new { controller = "Boards", action = "Forum", slug = UrlParameter.Optional }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ForumSlugPaged",
            //                "boards/forum/{id}/{slug}/page/{page}",
            //                new { controller = "Boards", action = "Forum", slug = UrlParameter.Optional, page = UrlParameter.Optional }));
            //                new { id = @"\w+", page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ForumGroupSlug",
            //                "boards/forumgroup/{id}/{slug}",
            //                new { controller = "Boards", action = "ForumGroup", slug = UrlParameter.Optional }));
            //                new { id = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Search",
                            "boards/search",
                            new { controller = "Boards", action = "Search" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //private messages
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PrivateMessages",
            //                "privatemessages/{tab}",
            //                new { controller = "PrivateMessages", action = "Index", tab = UrlParameter.Optional }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PrivateMessagesPaged",
            //                "privatemessages/{tab}/page/{page}",
            //                new { controller = "PrivateMessages", action = "Index", tab = UrlParameter.Optional }));
            //                new { page = @"\d+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PrivateMessagesInbox",
                            "inboxupdate",
                            new { controller = "PrivateMessages", action = "InboxUpdate" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PrivateMessagesSent",
                            "sentupdate",
                            new { controller = "PrivateMessages", action = "SentUpdate" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("SendPM",
            //                "sendpm/{toCustomerId}",
            //                new { controller = "PrivateMessages", action = "SendPM" }));
            //                new { toCustomerId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("SendPMReply",
            //                "sendpm/{toCustomerId}/{replyToMessageId}",
            //                new { controller = "PrivateMessages", action = "SendPM" }));
            //                new { toCustomerId = @"\w+", replyToMessageId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("ViewPM",
            //                "viewpm/{privateMessageId}",
            //                new { controller = "PrivateMessages", action = "ViewPM" }));
            //                new { privateMessageId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("DeletePM",
            //                "deletepm/{privateMessageId}",
            //                new { controller = "PrivateMessages", action = "DeletePM" }));
            //                new { privateMessageId = @"\w+" }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //activate newsletters
            //routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("NewsletterActivation",
            //                "newsletter/subscriptionactivation/{token}/{active}",
            //                new { controller = "Newsletter", action = "SubscriptionActivation" }));
            //                new { token = new GuidConstraint(false) }));
            //                /*new[] { "Grand.Web.Controllers" }));*/

            //robots.txt
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("robots.txt",
                            "robots.txt",
                            new { controller = "Common", action = "RobotsTextFile" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //sitemap (XML)
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("sitemap.xml",
                            "sitemap.xml",
                            new { controller = "Common", action = "SitemapXml" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //store closed
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("StoreClosed",
                            "storeclosed",
                            new { controller = "Common", action = "StoreClosed" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //install
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Installation",
                            "install",
                            new { controller = "Install", action = "Index" }));
            /*new[] { "Grand.Web.Controllers" }));*/
            //upgrade
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("Upgrade",
                            "upgrade",
                            new { controller = "Upgrade", action = "Index" }));
            /*new[] { "Grand.Web.Controllers" }));*/

            //page not found
            routes.Add(new NameTemplateDefaults/*routes.MapRoute*/("PageNotFound",
                            "page-not-found",
                            new { controller = "Common", action = "PageNotFound" }));
            /*new[] { "Grand.Web.Controllers" }));*/





            return routes;
            //}));
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
