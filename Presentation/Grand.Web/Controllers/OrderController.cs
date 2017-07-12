﻿//using System;
//using System.Collections.Generic;
//using System.IO;
//using Microsoft.AspNetCore.Mvc;
//using Grand.Core;
//using Grand.Core.Domain.Customers;
//using Grand.Core.Domain.Orders;
//using Grand.Core.Domain.Shipping;
//using Grand.Services.Common;
//using Grand.Services.Orders;
//using Grand.Services.Payments;
//using Grand.Services.Shipping;
//using Grand.Web.Framework.Controllers;
//using Grand.Web.Framework.Security;
//using Grand.Core.Infrastructure;
//using Grand.Web.Services;

//namespace Grand.Web.Controllers
//{
//    public partial class OrderController : BasePublicController
//    {
//        #region Fields

//        private readonly IOrderWebService _orderWebService;
//        private readonly IOrderService _orderService;
//        private readonly IWorkContext _workContext;
//        private readonly IOrderProcessingService _orderProcessingService;
//        private readonly IPaymentService _paymentService;
//        private readonly IWebHelper _webHelper;

//        #endregion

//		#region Constructors

//        public OrderController(IOrderWebService orderWebService,
//            IOrderService orderService,
//            IWorkContext workContext,
//            IOrderProcessingService orderProcessingService, 
//            IPaymentService paymentService, 
//            IWebHelper webHelper)
//        {
//            this._orderWebService = orderWebService;
//            this._orderService = orderService;
//            this._workContext = workContext;
//            this._orderProcessingService = orderProcessingService;
//            this._paymentService = paymentService;
//            this._webHelper = webHelper;
//        }

//        #endregion

//        #region Methods

//        //My account / Orders
//        //[GrandHttpsRequirement(SslRequirement.Yes)]
//        public virtual IActionResult CustomerOrders()
//        {
//            if (!_workContext.CurrentCustomer.IsRegistered())
//                return new UnauthorizedResult();

//            var model = _orderWebService.PrepareCustomerOrderList();
//            return View(model);
//        }

//        //My account / Orders / Cancel recurring order
//        [HttpPost, ActionName("CustomerOrders")]
//        //[PublicAntiForgery]
//        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
//        public virtual IActionResult CancelRecurringPayment(IFormCollection form)
//        {
//            if (!_workContext.CurrentCustomer.IsRegistered())
//                return new UnauthorizedResult();

//            //get recurring payment identifier
//            string recurringPaymentId = "";
//            foreach (var formValue in form.AllKeys)
//                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.OrdinalIgnoreCase))
//                    recurringPaymentId = formValue.Substring("cancelRecurringPayment".Length);

//            var recurringPayment = _orderService.GetRecurringPaymentById(recurringPaymentId);
//            if (recurringPayment == null)
//            {
//                return RedirectToRoute("CustomerOrders");
//            }

//            if (_orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment))
//            {
//                var errors = _orderProcessingService.CancelRecurringPayment(recurringPayment);

//                var model = _orderWebService.PrepareCustomerOrderList();
//                model.CancelRecurringPaymentErrors = errors;

//                return View(model);
//            }
//            else
//            {
//                return RedirectToRoute("CustomerOrders");
//            }
//        }

//        //My account / Reward points
//        //[GrandHttpsRequirement(SslRequirement.Yes)]
//        public virtual IActionResult CustomerRewardPoints()
//        {
//            if (!_workContext.CurrentCustomer.IsRegistered())
//                return new UnauthorizedResult();

//            var rewardPointsSettings = EngineContextExperimental.Current.Resolve<RewardPointsSettings>();
//            if (!rewardPointsSettings.Enabled)
//                return RedirectToRoute("CustomerInfo");

//            var customer = _workContext.CurrentCustomer;
//            var model = _orderWebService.PrepareCustomerRewardPoints(customer);
//            return View(model);
//        }

//        //My account / Order details page
//        //[GrandHttpsRequirement(SslRequirement.Yes)]
//        public virtual IActionResult Details(string orderId)
//        {
//            var order = _orderService.GetOrderById(orderId);
//            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
//                return new UnauthorizedResult();

//            var model = _orderWebService.PrepareOrderDetails(order);

//            return View(model);
//        }

//        //My account / Order details page / Print
//        //[GrandHttpsRequirement(SslRequirement.Yes)]
//        public virtual IActionResult PrintOrderDetails(string orderId)
//        {
//            var order = _orderService.GetOrderById(orderId);
//            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
//                return new UnauthorizedResult();

//            var model = _orderWebService.PrepareOrderDetails(order);
//            model.PrintMode = true;

//            return View("Details", model);
//        }

//        //My account / Order details page / Cancel Unpaid Order
//        //[GrandHttpsRequirement(SslRequirement.Yes)]
//        public IActionResult CancelOrder(string orderId)
//        {
//            var orderSettings = EngineContextExperimental.Current.Resolve<OrderSettings>();
//            var order = _orderService.GetOrderById(orderId);
//            if (order == null || order.PaymentStatus != Core.Domain.Payments.PaymentStatus.Pending
//                || (order.ShippingStatus != ShippingStatus.ShippingNotRequired && order.ShippingStatus != ShippingStatus.NotYetShipped)
//                || order.OrderStatus != OrderStatus.Pending
//                || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId
//                || !orderSettings.UserCanCancelUnpaidOrder)

//                return new UnauthorizedResult();

//            _orderProcessingService.CancelOrder(order, true, true);

//            return RedirectToRoute("OrderDetails", new { orderId = orderId });
//        }

//        //My account / Order details page / PDF invoice
//        public virtual IActionResult GetPdfInvoice(string orderId)
//        {
//            var order = _orderService.GetOrderById(orderId);
//            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
//                return new UnauthorizedResult();

//            var orders = new List<Order>();
//            orders.Add(order);
//            byte[] bytes;
//            using (var stream = new MemoryStream())
//            {
//                EngineContextExperimental.Current.Resolve<IPdfService>().PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
//                bytes = stream.ToArray();
//            }
//            return File(bytes, "application/pdf", string.Format("order_{0}.pdf", order.Id));
//        }

//        //My account / Order details page / re-order
//        public virtual IActionResult ReOrder(string orderId)
//        {
//            var order = _orderService.GetOrderById(orderId);
//            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
//                return new UnauthorizedResult();

//            _orderProcessingService.ReOrder(order);
//            return RedirectToRoute("ShoppingCart");
//        }

//        //My account / Order details page / Complete payment
//        [HttpPost, ActionName("Details")]
//        [FormValueRequired("repost-payment")]
//        //[PublicAntiForgery]
//        public virtual IActionResult RePostPayment(string orderId)
//        {
//            var order = _orderService.GetOrderById(orderId);
//            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
//                return new UnauthorizedResult();

//            if (!_paymentService.CanRePostProcessPayment(order))
//                return RedirectToRoute("OrderDetails", new { orderId = orderId });

//            var postProcessPaymentRequest = new PostProcessPaymentRequest
//            {
//                Order = order
//            };
//            _paymentService.PostProcessPayment(postProcessPaymentRequest);

//            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
//            {
//                //redirection or POST has been done in PostProcessPayment
//                return Content("Redirected");
//            }

//            //if no redirection has been done (to a third-party payment page)
//            //theoretically it's not possible
//            return RedirectToRoute("OrderDetails", new { orderId = orderId });
//        }

//        //My account / Order details page / Shipment details page
//        //[GrandHttpsRequirement(SslRequirement.Yes)]
//        public virtual IActionResult ShipmentDetails(string shipmentId)
//        {
//            var shipment = EngineContextExperimental.Current.Resolve<IShipmentService>().GetShipmentById(shipmentId);
//            if (shipment == null)
//                return new UnauthorizedResult();

//            var order = _orderService.GetOrderById(shipment.OrderId);
//            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
//                return new UnauthorizedResult();

//            var model = _orderWebService.PrepareShipmentDetails(shipment);

//            return View(model);
//        }

//        #endregion
//    }
//}
