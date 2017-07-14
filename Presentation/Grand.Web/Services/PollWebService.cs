using Grand.Core;
using Grand.Services.Localization;
using System;
using System.Linq;
using Grand.Core.Domain.Polls;
using Grand.Services.Polls;
using Grand.Web.Models.Polls;

namespace Grand.Web.Services
{
    public partial class PollWebService : IPollWebService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;

        #endregion

        #region Constructors

        public PollWebService(IWorkContext workContext, IPollService pollService)
        {
            this._workContext = workContext;
            this._pollService = pollService;
        }

        #endregion

        #region Utilities

        public virtual PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty)
        {
            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id),
                Name = poll.GetLocalized(x => x.Name)
            };
            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder);
            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;
            foreach (var pa in answers)
            {
                model.Answers.Add(new PollAnswerModel
                {
                    Id = pa.Id,
                    PollId = poll.Id,
                    Name = pa.GetLocalized(x => x.Name),
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0,
                });
            }
            return model;
        }

        #endregion
    }
}