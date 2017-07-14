﻿//using Grand.Admin.Models.Messages;
//using Grand.Services.Localization;
//using Grand.Services.Messages;
//using Grand.Services.Security;
//using Grand.Web.Framework.Kendoui;
//using Grand.Core.Domain.Localization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Grand.Core.Domain.Messages;
//using Grand.Web.Framework.Controllers;
//using Grand.Admin.Extensions;
//using Grand.Services.Stores;

//namespace Grand.Admin.Controllers
//{
//    public partial class NewsletterCategoryController: BaseAdminController
//    {
//        #region Fields 

//        private readonly IPermissionService _permissionService;
//        private readonly INewsletterCategoryService _newsletterCategoryService;
//        private readonly ILanguageService _languageService;
//        private readonly ILocalizationService _localizationService;
//        private readonly IStoreService _storeService;

//        #endregion

//        #region Ctor

//        public NewsletterCategoryController(IPermissionService permissionService, INewsletterCategoryService newsletterCategoryService, ILanguageService languageService,
//            ILocalizationService localizationService, IStoreService storeService)
//        {
//            this._permissionService = permissionService;
//            this._newsletterCategoryService = newsletterCategoryService;
//            this._languageService = languageService;
//            this._localizationService = localizationService;
//            this._storeService = storeService;
//        }

//        #endregion

//        #region Utilities

//        [NonAction]
//        protected virtual List<LocalizedProperty> UpdateLocales(NewsletterCategory newsletterCategory, NewsletterCategoryModel model)
//        {
//            List<LocalizedProperty> localized = new List<LocalizedProperty>();
//            foreach (var local in model.Locales)
//            {
//                localized.Add(new LocalizedProperty()
//                {
//                    LanguageId = local.LanguageId,
//                    LocaleKey = "Name",
//                    LocaleValue = local.Name
//                });

//                localized.Add(new LocalizedProperty()
//                {
//                    LanguageId = local.LanguageId,
//                    LocaleKey = "Description",
//                    LocaleValue = local.Description
//                });
//            }
//            return localized;
//        }

//        [NonAction]
//        protected virtual void PrepareStoresMappingModel(NewsletterCategoryModel model, NewsletterCategory newsletterCategory, bool excludeProperties)
//        {
//            if (model == null)
//                throw new ArgumentNullException("model");

//            model.AvailableStores = _storeService.GetAllStores().Select(s => s.ToModel()).ToList();

//            if (!excludeProperties)
//            {
//                if (newsletterCategory != null)
//                {
//                    model.SelectedStoreIds = newsletterCategory.Stores.ToArray();
//                }
//            }
//        }
//        #endregion

//        #region Methods

//        public ActionResult Index()
//        {
//            return RedirectToAction("List");
//        }

//        public ActionResult List()
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
//                return AccessDeniedView();

//            return View();
//        }

//        [HttpPost]
//        public ActionResult List(DataSourceRequest command)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
//                return AccessDeniedView();

//            var newslettercategories = _newsletterCategoryService.GetAllNewsletterCategory();
//            var gridModel = new DataSourceResult
//            {
//                Data = newslettercategories.Select(x =>
//                {
//                    return new {
//                        Id = x.Id,
//                        Name = x.Name,
//                        Selected = x.Selected
//                    };
//                }),
//                Total = newslettercategories.Count
//            };
//            return Json(gridModel);
//        }

//        public ActionResult Create()
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
//                return AccessDeniedView();

//            var model = new NewsletterCategoryModel();
//            //locales
//            AddLocales(_languageService, model.Locales);
//            //Stores
//            PrepareStoresMappingModel(model, null, false);

//            return View(model);
//        }

//        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
//        public ActionResult Create(NewsletterCategoryModel model, bool continueEditing)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
//                return AccessDeniedView();

//            if (ModelState.IsValid)
//            {
//                var newsletterCategory = model.ToEntity();
//                newsletterCategory.Locales = UpdateLocales(newsletterCategory, model);
//                newsletterCategory.Stores = model.SelectedStoreIds != null ? model.SelectedStoreIds.ToList() : new List<string>();
//                _newsletterCategoryService.InsertNewsletterCategory(newsletterCategory);

//                SuccessNotification(_localizationService.GetResource("Admin.Promotions.NewsletterCategory.Added"));
//                return continueEditing ? RedirectToAction("Edit", new { id = newsletterCategory.Id }) : RedirectToAction("List");
//            }

//            //Stores
//            PrepareStoresMappingModel(model, null, false);

//            return View(model);
//        }

//        public ActionResult Edit(string id)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
//                return AccessDeniedView();

//            var newsletterCategory = _newsletterCategoryService.GetNewsletterCategoryById(id);
//            if (newsletterCategory == null)
//                return RedirectToAction("List");

//            var model = newsletterCategory.ToModel();

//            //locales
//            AddLocales(_languageService, model.Locales, (locale, languageId) =>
//            {
//                locale.Name = newsletterCategory.GetLocalized(x => x.Name, languageId, false, false);
//                locale.Description = newsletterCategory.GetLocalized(x => x.Description, languageId, false, false);
//            });

//            //Stores
//            PrepareStoresMappingModel(model, newsletterCategory, false);

//            return View(model);
//        }

//        [HttpPost]
//        [ParameterBasedOnFormName("save-continue", "continueEditing")]
//        [FormValueRequired("save", "save-continue")]
//        public ActionResult Edit(NewsletterCategoryModel model, bool continueEditing)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNewsletterSubscribers))
//                return AccessDeniedView();

//            var newsletterCategory = _newsletterCategoryService.GetNewsletterCategoryById(model.Id);
//            if (newsletterCategory == null)
//                return RedirectToAction("List");

//            if (ModelState.IsValid)
//            {
//                newsletterCategory = model.ToEntity(newsletterCategory);
//                newsletterCategory.Locales = UpdateLocales(newsletterCategory, model);
//                newsletterCategory.Stores = model.SelectedStoreIds != null ? model.SelectedStoreIds.ToList() : new List<string>();
//                _newsletterCategoryService.UpdateNewsletterCategory(newsletterCategory);

//                SuccessNotification(_localizationService.GetResource("Admin.Promotions.NewsletterCategory.Updated"));
//                return continueEditing ? RedirectToAction("Edit", new { id = newsletterCategory.Id }) : RedirectToAction("List");
//            }
//            //Stores
//            PrepareStoresMappingModel(model, newsletterCategory, true);

//            return View(model);
//        }

//        [HttpPost]
//        public ActionResult Delete(string id)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
//                return AccessDeniedView();

//            var newsletterCategory = _newsletterCategoryService.GetNewsletterCategoryById(id);
//            if (newsletterCategory == null)
//                return RedirectToAction("List");

//            _newsletterCategoryService.DeleteNewsletterCategory(newsletterCategory);

//            SuccessNotification(_localizationService.GetResource("Admin.Promotions.NewsletterCategory.Deleted"));
//            return RedirectToAction("List");
//        }

//        #endregion


//    }
//}