using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Polls;
using Grand.Services.Localization;
using Grand.Services.Polls;
using Grand.Web.Infrastructure.Cache;
using Grand.Web.Models.Polls;
using Grand.Core.Infrastructure;
using Grand.Services.Customers;
using Grand.Services.Security;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Grand.Web.Controllers;
using Grand.Web.Services;

namespace Grand.Web.ViewComponents
{
    public class PollViewComponent : ViewComponent
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IAclService _aclService;
        private readonly IPollWebService _pollWebService;

        #endregion

        #region Constructors

        public PollViewComponent(
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPollService pollService,
            //ICacheManager cacheManager,
            IStoreContext storeContext,
            IAclService aclService,
            IPollWebService pollWebService)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._pollService = pollService;
            //this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._aclService = aclService;
            this._pollWebService = pollWebService;
        }

        #endregion

        #region Invoker
        
        public async Task<IViewComponentResult> InvokeAsync(string actionName, string systemKeyword)
        {
            switch (actionName)
            {
                case nameof(this.PollBlock):
                    return PollBlock(systemKeyword);
                case nameof(this.HomePagePolls):
                    return HomePagePolls();
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
            
        }
        
        #endregion

        #region Methods

        public virtual /*Task<*/IViewComponentResult PollBlock(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return Content("");

            var cacheKey = string.Format(ModelCacheEventConsumer.POLL_BY_SYSTEMNAME__MODEL_KEY, systemKeyword, _storeContext.CurrentStore.Id);




            //just workaround i need
            Poll poll = _pollService.GetPollBySystemKeyword(systemKeyword, _storeContext.CurrentStore.Id);
            var cachedModel = _pollWebService.PreparePollModel(poll, false);




            //var cachedModel = _cacheManager.Get(cacheKey, () =>
            //{
            //    Poll poll = _pollService.GetPollBySystemKeyword(systemKeyword, _storeContext.CurrentStore.Id);
            //    //ACL (access control list)
            //    if (!_aclService.Authorize(poll))
            //        return new PollModel { Id = "" };
            //    if (poll == null ||
            //        !poll.Published ||
            //        (poll.StartDateUtc.HasValue && poll.StartDateUtc.Value > DateTime.UtcNow) ||
            //        (poll.EndDateUtc.HasValue && poll.EndDateUtc.Value < DateTime.UtcNow))
            //        //we do not cache nulls. that's why let's return an empty record (ID = 0)
            //        return new PollModel { Id = "" };
            //    return _pollWebService.PreparePollModel(poll, false);
            //});

            if (cachedModel == null || cachedModel.Id == "")
                return Content("");

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (PollModel)cachedModel.Clone();
            model.AlreadyVoted = _pollService.AlreadyVoted(model.Id, _workContext.CurrentCustomer.Id);

            return View(nameof(this.PollBlock), model);
        }
        
        public virtual /*Task<*/IViewComponentResult HomePagePolls()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_POLLS_MODEL_KEY, _workContext.WorkingLanguage.Id);
            var cachedModel = /*_cacheManager.Get(cacheKey, () =>*/
                _pollService.GetPolls(_storeContext.CurrentStore.Id, true)
                .Select(x => _pollWebService.PreparePollModel(x, false))
                .ToList()/*)*/;
            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var p in cachedModel)
            {
                var pollModel = (PollModel)p.Clone();
                pollModel.AlreadyVoted = _pollService.AlreadyVoted(pollModel.Id, _workContext.CurrentCustomer.Id);
                model.Add(pollModel);
            }

            if (!model.Any())
                Content("");
            return View(nameof(this.HomePagePolls), model);
        }

        #endregion

    }
}
