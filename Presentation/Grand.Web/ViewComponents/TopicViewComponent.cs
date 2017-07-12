using System;
using System.Threading.Tasks;
using Grand.Services.Localization;
using Grand.Services.Security;
using Grand.Services.Stores;
using Grand.Services.Topics;
using Grand.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.ViewComponents
{
    public class TopicViewComponent : ViewComponent
    {
        #region Fields
        private readonly ITopicWebService _topicWebService;
        private readonly ITopicService _topicService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public TopicViewComponent(ITopicService topicService,
            ITopicWebService topicWebService,
            ILocalizationService localizationService,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            IPermissionService permissionService)
        {
            this._topicService = topicService;
            this._topicWebService = topicWebService;
            this._localizationService = localizationService;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string actionName, string systemName)
        {
            switch (actionName)
            {
                case nameof(this.TopicBlock):
                    return await TopicBlock(systemName);
                default:
                    throw new InvalidOperationException(nameof(this.InvokeAsync));
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IViewComponentResult> TopicBlock(string systemName)
        {
            var model = _topicWebService.TopicBlock(systemName);
            if (model == null)
                return Content("");

            return View(nameof(this.TopicBlock), model);
        }

        #endregion
    }
}