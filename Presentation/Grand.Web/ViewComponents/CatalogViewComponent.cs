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
/*using System.Web.Mvc;*/
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
    public class /*Experimental*/CatalogViewComponent : ViewComponent
    {
        private readonly ICatalogWebService _catalogWebService;

        public /*Experimental*/CatalogViewComponent(
            ICatalogWebService catalogWebService
            /*ToDoContext context*/)
        {
            this._catalogWebService = catalogWebService;

            //db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string actionName)
        {
            //no use of Controller inside ViewComponent, this will be misunderstood of concept
            //var qq = this.HttpContext.RequestServices.GetService(typeof(Grand.Web.Controllers.CatalogController));// as Controllers.CatalogController;



            //just try
            //var items = new List<string> { "one", "two", "three", "four", "fajv" };
            //return View("TopMenu", items);



            if (actionName == nameof(this.TopMenu))
               return await TopMenu();
            if (actionName == nameof(this.HomepageCategories))
                return await HomepageCategories();



            //previous working
            //if (actionName == "TopMenu")
            //{
            //    var model = _catalogWebService.PrepareTopMenu();
            //    return View("TopMenu", model);
            //}

            return Content("sometimes you run so fast, you lose your aim out of sight");


            //var qqqq = this.View("TopMenu");
            //return qqqq;

            ////var result = qq.TopMenu();


            //var items = new List<string> { "one", "two", "three", "four", "fajv" };// await GetItemsAsync(maxPriority, isDone);
            //return View(qqqq);
        }

        public virtual async Task<IViewComponentResult>/*IActionResult*/ TopMenu()
        {
            var model = _catalogWebService.PrepareTopMenu();
            return View("TopMenu", model);

            

            //var model = _catalogWebService.PrepareTopMenu();
            //return PartialView(model);
        }


        public virtual async Task<IViewComponentResult>/*IActionResult*/ HomepageCategories()
        {
            var model = _catalogWebService.PrepareHomepageCategory();
            if (!model.Any())
                return Content("");

            return View("HomepageCategories", model);
        }
    }
}
