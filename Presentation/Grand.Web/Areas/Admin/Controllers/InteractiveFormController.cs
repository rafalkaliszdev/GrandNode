﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using Grand.Admin.Extensions;
//using Grand.Admin.Models.Messages;
//using Grand.Core.Domain.Messages;
//using Grand.Services.Localization;
//using Grand.Services.Messages;
//using Grand.Services.Security;
//using Grand.Web.Framework.Controllers;
//using Grand.Web.Framework.Kendoui;
//using Grand.Core.Domain.Localization;
//using Grand.Web.Framework.Mvc;
//using Grand.Services.Logging;
//using System.Text;

//namespace Grand.Admin.Controllers
//{
//    public partial class InteractiveFormController : BaseAdminController
//    {
//        private readonly IInteractiveFormService _interactiveFormService;
//        private readonly IPermissionService _permissionService;
//        private readonly ILocalizationService _localizationService;
//        private readonly ILanguageService _languageService;
//        private readonly ICustomerActivityService _customerActivityService;
//        private readonly IEmailAccountService _emailAccountService;

//        public InteractiveFormController(IInteractiveFormService interactiveFormService,
//            ILocalizationService localizationService,
//            IPermissionService permissionService,
//            ILanguageService languageService,
//            ICustomerActivityService customerActivityService,
//            IEmailAccountService emailAccountService)
//        {
//            this._interactiveFormService = interactiveFormService;
//            this._localizationService = localizationService;
//            this._permissionService = permissionService;
//            this._languageService = languageService;
//            this._customerActivityService = customerActivityService;
//            this._emailAccountService = emailAccountService;
//        }

//        #region Utilities

//        [NonAction]
//        protected virtual List<LocalizedProperty> UpdateLocales(InteractiveForm iform, InteractiveFormModel model)
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
//                    LocaleKey = "Body",
//                    LocaleValue = local.Body
//                });
//            }
//            return localized;
//        }
//        [NonAction]
//        protected virtual List<LocalizedProperty> UpdateLocales(InteractiveForm.FormAttribute attribute, InteractiveFormAttributeModel model)
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
//            }
//            return localized;
//        }
//        [NonAction]
//        protected virtual List<LocalizedProperty> UpdateLocales(InteractiveForm.FormAttributeValue attributevalue, InteractiveFormAttributeValueModel model)
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
//            }
//            return localized;
//        }

//        private string FormatTokens(string[] tokens)
//        {
//            var sb = new StringBuilder();
//            for (int i = 0; i < tokens.Length; i++)
//            {
//                string token = tokens[i];
//                sb.Append(token);
//                if (i != tokens.Length - 1)
//                    sb.Append(", ");
//            }

//            return sb.ToString();
//        }
//        #endregion

//        #region List forms

//        public ActionResult Index()
//        {
//            return RedirectToAction("List");
//        }

//        public ActionResult List()
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            return View();
//        }

//        [HttpPost]
//        public ActionResult List(DataSourceRequest command)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
//                return AccessDeniedView();

//            var forms = _interactiveFormService.GetAllForms();
//            var gridModel = new DataSourceResult
//            {
//                Data = forms.Select(x =>
//                {
//                    var model = x.ToModel();
//                    model.Body = "";
//                    return model;
//                }),
//                Total = forms.Count
//            };
//            return Json(gridModel);
//        }
//        #endregion

//        #region Update form

//        public ActionResult Create()
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var model = new InteractiveFormModel();
//            //locales
//            AddLocales(_languageService, model.Locales);
//            //available email accounts
//            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
//                model.AvailableEmailAccounts.Add(ea.ToModel());

//            return View(model);
//        }

//        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
//        public ActionResult Create(InteractiveFormModel model, bool continueEditing)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            if (ModelState.IsValid)
//            {
//                var form = model.ToEntity();
//                form.CreatedOnUtc = DateTime.UtcNow;
//                form.Locales = UpdateLocales(form, model);

//                _interactiveFormService.InsertForm(form);
//                _customerActivityService.InsertActivity("InteractiveFormAdd", form.Id, _localizationService.GetResource("ActivityLog.InteractiveFormAdd"), form.Name);

//                SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Added"));
//                return continueEditing ? RedirectToAction("Edit", new { id = form.Id }) : RedirectToAction("List");
//            }
//            //locales
//            AddLocales(_languageService, model.Locales);
//            //available email accounts
//            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
//                model.AvailableEmailAccounts.Add(ea.ToModel());
//            return View(model);
//        }

//        public ActionResult Edit(string id)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(id);
//            if (form == null)
//                return RedirectToAction("List");

//            var model = form.ToModel();

//            AddLocales(_languageService, model.Locales, (locale, languageId) =>
//            {
//                locale.Name = form.GetLocalized(x => x.Name, languageId, false, false);
//                locale.Body = form.GetLocalized(x => x.Body, languageId, false, false);
//            });
//            //available email accounts
//            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
//                model.AvailableEmailAccounts.Add(ea.ToModel());

//            //available tokens
//            model.AvailableTokens = FormatTokens(form.FormAttributes.Select(x => "%" + x.SystemName + "%").ToArray());
//            return View(model);
//        }

//        [HttpPost]
//        [ParameterBasedOnFormName("save-continue", "continueEditing")]
//        [FormValueRequired("save", "save-continue")]
//        public ActionResult Edit(InteractiveFormModel model, bool continueEditing)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(model.Id);
//            if (form == null)
//                return RedirectToAction("List");

//            if (ModelState.IsValid)
//            {
//                form = model.ToEntity(form);
//                form.Locales = UpdateLocales(form, model);
//                _interactiveFormService.UpdateForm(form);

//                _customerActivityService.InsertActivity("InteractiveFormEdit", form.Id, _localizationService.GetResource("ActivityLog.InteractiveFormUpdate"), form.Name);

//                SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Updated"));
//                return continueEditing ? RedirectToAction("Edit", new { id = form.Id }) : RedirectToAction("List");
//            }

//            AddLocales(_languageService, model.Locales, (locale, languageId) =>
//            {
//                locale.Name = form.GetLocalized(x => x.Name, languageId, false, false);
//                locale.Body = form.GetLocalized(x => x.Body, languageId, false, false);
//            });
//            //available email accounts
//            foreach (var ea in _emailAccountService.GetAllEmailAccounts())
//                model.AvailableEmailAccounts.Add(ea.ToModel());

//            //available tokens
//            model.AvailableTokens = FormatTokens(form.FormAttributes.Select(x => "%" + x.SystemName + "%").ToArray());

//            return View(model);
//        }

//        [HttpPost]
//        public ActionResult Delete(string id)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(id);
//            if (form == null)
//                return RedirectToAction("List");

//            _interactiveFormService.DeleteForm(form);

//            _customerActivityService.InsertActivity("InteractiveFormDelete", form.Id, _localizationService.GetResource("ActivityLog.InteractiveFormDeleted"), form.Name);

//            SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Deleted"));
//            return RedirectToAction("List");
//        }
//        #endregion

//        #region Form attributes

//        [HttpPost]
//        public ActionResult FormAttributesList(string formId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(formId);
//            if (form == null)
//                return RedirectToAction("List");

//            var gridModel = new DataSourceResult
//            {
//                Data = form.FormAttributes.Select(x => new
//                {
//                    Id = x.Id,
//                    Name = x.Name,
//                    SystemName = x.SystemName,
//                    Type = ((FormControlType)x.FormControlTypeId).ToString()
//                }),
//                Total = form.FormAttributes.Count()
//            };
//            return Json(gridModel);
//        }

//        [HttpPost]
//        public ActionResult FormAttributesDelete(string id, string formId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(formId);
//            if (form == null)
//                return RedirectToAction("List");

//            form.FormAttributes.Remove(form.FormAttributes.FirstOrDefault(x => x.Id == id));

//            _interactiveFormService.UpdateForm(form);

//            _customerActivityService.InsertActivity("InteractiveFormEdit", form.Id, _localizationService.GetResource("ActivityLog.InteractiveFormDeleteAttribute"), form.Name);


//            return new NullJsonResult();
//        }

//        #region Form Attributes

//        public ActionResult AddAttribute(string formId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            InteractiveFormAttributeModel model = new InteractiveFormAttributeModel();
//            model.FormId = formId;

//            AddLocales(_languageService, model.Locales);
//            return View(model);
//        }

//        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
//        public ActionResult AddAttribute(InteractiveFormAttributeModel model, bool continueEditing)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            if (ModelState.IsValid)
//            {
//                var form = _interactiveFormService.GetFormById(model.FormId);
//                if (form == null)
//                {
//                    return RedirectToAction("List");
//                }
//                var attribute = model.ToEntity();
//                attribute.Locales = UpdateLocales(attribute, model);
//                form.FormAttributes.Add(attribute);
//                _interactiveFormService.UpdateForm(form);

//                _customerActivityService.InsertActivity("InteractiveFormEdit", attribute.Id, _localizationService.GetResource("ActivityLog.InteractiveFormAddAttribute"), attribute.Name);

//                SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Attribute.Added"));

//                return continueEditing ? RedirectToAction("EditAttribute", new { formId = form.Id, aid = attribute.Id }) : RedirectToAction("Edit", new { id = form.Id });
//            }

//            return View(model);
//        }

//        public ActionResult EditAttribute(string formId, string aid)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(formId);
//            if (form == null)
//                return RedirectToAction("List");

//            var attribute = form.FormAttributes.FirstOrDefault(x => x.Id == aid);
//            if (attribute == null)
//                return RedirectToAction("List");

//            var model = attribute.ToModel();
//            model.FormId = formId;

//            AddLocales(_languageService, model.Locales, (locale, languageId) =>
//            {
//                locale.Name = attribute.GetLocalized(x => x.Name, languageId, false, false);
//            });

//            return View(model);
//        }

//        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
//        public ActionResult EditAttribute(string formId, InteractiveFormAttributeModel model, bool continueEditing)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(formId);
//            if (form == null)
//                return RedirectToAction("List");

//            var attribute = form.FormAttributes.FirstOrDefault(x => x.Id == model.Id);
//            if (attribute == null)
//                return RedirectToAction("List");
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    attribute = model.ToEntity(attribute);
//                    attribute.Locales = UpdateLocales(attribute, model);
//                    _interactiveFormService.UpdateForm(form);

//                    _customerActivityService.InsertActivity("InteractiveFormEdit", attribute.Id, _localizationService.GetResource("ActivityLog.InteractiveFormUpdateAttribute"), attribute.Name);

//                    SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Attribute.Updated"));
//                    return continueEditing ? RedirectToAction("EditAttribute", new { formId = form.Id, aid = attribute.Id }) : RedirectToAction("Edit", new { id = form.Id });
//                }
//                return View(model);
//            }
//            catch (Exception exc)
//            {
//                ErrorNotification(exc);
//                return RedirectToAction("Edit", new { id = form.Id });
//            }
//        }

//        #endregion

//        #region Attribute Value

//        [HttpPost]
//        public ActionResult AttributeValuesList(string formId, string aId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();
//            var form = _interactiveFormService.GetFormById(formId);
//            if (form == null)
//                return RedirectToAction("List");
//            var values = form.FormAttributes.FirstOrDefault(x => x.Id == aId).FormAttributeValues;
//            var gridModel = new DataSourceResult
//            {
//                Data = values.Select(x => new
//                {
//                    Id = x.Id,
//                    Name = x.Name,
//                    IsPreSelected = x.IsPreSelected,
//                    DisplayOrder = x.DisplayOrder,
//                }),
//                Total = values.Count()
//            };
//            return Json(gridModel);
//        }

//        public ActionResult ValueCreatePopup(string form, string aId, string btnId, string formId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var fo = _interactiveFormService.GetFormById(form);
//            if (fo == null)
//                return RedirectToAction("List");
//            var attribute = fo.FormAttributes.FirstOrDefault(x => x.Id == aId);
//            if (attribute == null)
//                return RedirectToAction("List");

//            var model = new InteractiveFormAttributeValueModel();
//            model.FormId = fo.Id;
//            model.AttributeId = aId;

//            ViewBag.RefreshPage = false;
//            ViewBag.btnId = btnId;
//            ViewBag.formId = formId;

//            AddLocales(_languageService, model.Locales);
//            return View(model);
//        }

//        [HttpPost]
//        public ActionResult ValueCreatePopup(string btnId, string formId, string form, string aId, InteractiveFormAttributeValueModel model)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var fo = _interactiveFormService.GetFormById(form);
//            if (fo == null)
//                return RedirectToAction("List");
//            var attribute = fo.FormAttributes.FirstOrDefault(x => x.Id == aId);
//            if (attribute == null)
//                return RedirectToAction("List");

//            if (ModelState.IsValid)
//            {
//                var vaf = new InteractiveForm.FormAttributeValue
//                {
//                    Name = model.Name,
//                    IsPreSelected = model.IsPreSelected,
//                    DisplayOrder = model.DisplayOrder,
//                };
//                vaf.Locales = UpdateLocales(vaf, model);
//                attribute.FormAttributeValues.Add(vaf);
//                _interactiveFormService.UpdateForm(fo);
//                _customerActivityService.InsertActivity("InteractiveFormEdit", vaf.Id, _localizationService.GetResource("ActivityLog.InteractiveFormAddAttributeValue"), vaf.Name);

//                ViewBag.RefreshPage = true;
//                ViewBag.btnId = btnId;
//                ViewBag.formId = formId;
//                return View(model);
//            }

//            return View(model);
//        }

//        //edit
//        public ActionResult ValueEditPopup(string id, string form, string aId, string formId, string btnId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var fo = _interactiveFormService.GetFormById(form);
//            if (fo == null)
//                return RedirectToAction("List");
//            var attribute = fo.FormAttributes.FirstOrDefault(x => x.Id == aId);
//            if (attribute == null)
//                return RedirectToAction("List");

//            var vaf = attribute.FormAttributeValues.Where(x => x.Id == id).FirstOrDefault();
//            if (vaf == null)
//                return RedirectToAction("List");

//            var model = new InteractiveFormAttributeValueModel
//            {
//                Id = vaf.Id,
//                Name = vaf.Name,
//                IsPreSelected = vaf.IsPreSelected,
//                DisplayOrder = vaf.DisplayOrder,
//                FormId = fo.Id,
//                AttributeId = aId,
//            };

//            AddLocales(_languageService, model.Locales, (locale, languageId) =>
//            {
//                locale.Name = vaf.GetLocalized(x => x.Name, languageId, false, false);
//            });
//            ViewBag.RefreshPage = false;
//            ViewBag.btnId = btnId;
//            ViewBag.formId = formId;
//            return View(model);
//        }

//        [HttpPost]
//        public ActionResult ValueEditPopup(string btnId, string formId, string form, string aId, InteractiveFormAttributeValueModel model)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var fo = _interactiveFormService.GetFormById(form);
//            if (fo == null)
//                return RedirectToAction("List");
//            var attribute = fo.FormAttributes.FirstOrDefault(x => x.Id == aId);
//            if (attribute == null)
//                return RedirectToAction("List");

//            var vaf = attribute.FormAttributeValues.Where(x => x.Id == model.Id).FirstOrDefault();
//            if (vaf == null)
//                return RedirectToAction("List");

//            if (ModelState.IsValid)
//            {
//                vaf.Name = model.Name;
//                vaf.IsPreSelected = model.IsPreSelected;
//                vaf.DisplayOrder = model.DisplayOrder;
//                vaf.Locales = UpdateLocales(vaf, model);
//                _interactiveFormService.UpdateForm(fo);

//                _customerActivityService.InsertActivity("InteractiveFormEdit", vaf.Id, _localizationService.GetResource("ActivityLog.InteractiveFormUpdateAttributeValue"), vaf.Name);

//                ViewBag.RefreshPage = true;
//                ViewBag.btnId = btnId;
//                ViewBag.formId = formId;
//                return View(model);
//            }

//            return View(model);
//        }

//        [HttpPost]
//        public ActionResult AttributeValuesDelete(string id, string formId, string aId)
//        {
//            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInteractiveForm))
//                return AccessDeniedView();

//            var form = _interactiveFormService.GetFormById(formId);
//            if (form == null)
//                return RedirectToAction("List");
//            var attribute = form.FormAttributes.FirstOrDefault(x => x.Id == aId);
//            if (attribute == null)
//                return RedirectToAction("List");

//            var vaf = attribute.FormAttributeValues.Where(x => x.Id == id).FirstOrDefault();
//            if (vaf == null)
//                return RedirectToAction("List");

//            attribute.FormAttributeValues.Remove(vaf);
//            _interactiveFormService.UpdateForm(form);

//            _customerActivityService.InsertActivity("InteractiveFormEdit", vaf.Id, _localizationService.GetResource("ActivityLog.InteractiveFormDeleteAttributeValue"), vaf.Name);

//            return new NullJsonResult();
//        }

//        #endregion


//        #endregion

//    }
//}
