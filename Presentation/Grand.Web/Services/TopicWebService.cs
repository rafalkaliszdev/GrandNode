﻿using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Topics;
using Grand.Services.Customers;
using Grand.Services.Localization;
using Grand.Services.Security;
using Grand.Services.Seo;
using Grand.Services.Stores;
using Grand.Services.Topics;
using Grand.Web.Infrastructure.Cache;
using Grand.Web.Models.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Web;*/

namespace Grand.Web.Services
{
    public partial class TopicWebService : ITopicWebService
    {
        public readonly ITopicService _topicService;
        public readonly IWorkContext _workContext;
        public readonly IStoreContext _storeContext;
        public readonly ICacheManager _cacheManager;
        public readonly ITopicTemplateService _topicTemplateService;
        public readonly IStoreMappingService _storeMappingService;
        public readonly IAclService _aclService;

        public TopicWebService(ITopicService topicService,
            IWorkContext workContext,
            IStoreContext storeContext,
            //ICacheManager cacheManager,
            ITopicTemplateService topicTemplateService,
            IStoreMappingService storeMappingService,
            IAclService aclService)
        {
            this._topicService = topicService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            //this._cacheManager = cacheManager;
            this._topicTemplateService = topicTemplateService;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
        }
        public virtual TopicModel PrepareTopicModel(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            var model = new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
                SeName = topic.GetSeName(),
                TopicTemplateId = topic.TopicTemplateId
            };
            return model;

        }
        public virtual TopicModel TopicDetails(string topicId)
        {

            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_ID_KEY,
                topicId,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
                {
                    var topic = _topicService.GetTopicById(topicId);
                    if (topic == null)
                        return null;
                    //Store mapping
                    if (!_storeMappingService.Authorize(topic))
                        return null;
                    //ACL (access control list)
                    if (!_aclService.Authorize(topic))
                        return null;

                    return PrepareTopicModel(topic);
                }
            );

            return cacheModel;

        }
        public virtual TopicModel TopicDetailsPopup(string systemName)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_SYSTEMNAME_KEY,
                systemName,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));

            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                //load by store
                var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
                if (topic == null)
                    return null;
                //ACL (access control list)
                if (!_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            });
            return cacheModel;
        }
        public virtual TopicModel TopicBlock(string systemName)
        {
            //woa
            return new TopicModel()
            {
                Body = "Proin nec sem vel nibh luctus gravida et et est. Suspendisse ut ligula non ligula tincidunt porta. Etiam purus elit, accumsan at consequat tempus, sollicitudin ac ex. Donec efficitur dolor sit amet mauris placerat ullamcorper. Aliquam erat volutpat. Suspendisse potenti. Cras tristique eros elit, et sagittis tortor auctor in. Maecenas volutpat ornare maximus. Aenean a tristique libero.Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Proin varius tristique nulla, in ultrices nunc pellentesque eu. Cras tortor augue, pretium a lectus eu, pretium viverra leo. Etiam molestie eget mi ac dictum. Praesent ac euismod ipsum. Aenean commodo turpis mauris, in gravida justo euismod egestas. Quisque euismod ipsum eget ornare luctus. Proin congue lectus id orci pellentesque, vel suscipit eros feugiat.",
                Title = "custom title"

            };

            var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_BY_SYSTEMNAME_KEY,
                systemName,
                _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                //load by store
                var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
                if (topic == null)
                    return null;
                //Store mapping
                if (!_storeMappingService.Authorize(topic))
                    return null;
                //ACL (access control list)
                if (!_aclService.Authorize(topic))
                    return null;
                return PrepareTopicModel(topic);
            });

            return cacheModel;
        }
        public virtual string PrepareTopicTemplateViewPath(string templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TEMPLATE_MODEL_KEY, templateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _topicTemplateService.GetTopicTemplateById(templateId);
                if (template == null)
                    template = _topicTemplateService.GetAllTopicTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });
            return templateViewPath;
        }
    }
}