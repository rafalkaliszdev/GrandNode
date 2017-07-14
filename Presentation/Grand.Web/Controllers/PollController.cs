using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Polls;
using Grand.Services.Localization;
using Grand.Services.Polls;
using Grand.Core.Infrastructure;
using Grand.Services.Customers;
using Grand.Services.Security;
using Grand.Web.Services;

namespace Grand.Web.Controllers
{
    public partial class PollController : BasePublicController
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

        public PollController(ILocalizationService localizationService,
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

        #region Methods

        [HttpPost]
        //[ValidateInput(false)]
        public virtual IActionResult Vote(string pollAnswerId, string pollId)
        {
            var poll = _pollService.GetPollById(pollId); //pollAnswer.Poll;
            if (!poll.Published)
                return Json(new
                {
                    error = "Poll is not available",
                });

            var pollAnswer = poll.PollAnswers.FirstOrDefault(x => x.Id == pollAnswerId);
            if (pollAnswer == null)
                return Json(new
                {
                    error = "No poll answer found with the specified id",
                });


            if (_workContext.CurrentCustomer.IsGuest() && !poll.AllowGuestsToVote)
                return Json(new
                {
                    error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote"),
                });

            bool alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id);
            if (!alreadyVoted)
            {
                //vote
                pollAnswer.PollVotingRecords.Add(new PollVotingRecord
                {
                    PollId = pollId,
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
                //update totals
                pollAnswer.NumberOfVotes = pollAnswer.PollVotingRecords.Count;
                _pollService.UpdatePoll(poll);

                if (!_workContext.CurrentCustomer.IsHasPoolVoting)
                {
                    _workContext.CurrentCustomer.IsHasPoolVoting = true;
                    EngineContextExperimental.Current.Resolve<ICustomerService>().UpdateHasPoolVoting(_workContext.CurrentCustomer.Id);
                }
            }

            return Json(new
            {
                html = this.RenderPartialViewToString("_Poll", _pollWebService.PreparePollModel(poll, true)),
            });
        }

        #endregion
    }
}