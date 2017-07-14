using Grand.Core.Domain.Polls;
using Grand.Web.Models.Polls;

namespace Grand.Web.Services
{
    public interface IPollWebService
    {
        PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty);
    }
}
