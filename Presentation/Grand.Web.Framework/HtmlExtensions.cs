﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Mvc.Html;
//using System.Web.WebPages;
using Grand.Core;
using Grand.Core.Infrastructure;
using Grand.Services.Localization;
using Grand.Services.Stores;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;
//using System.Web.Routing;
using Microsoft.AspNetCore.DiagnosticsViewPage.Views;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.IO;
using System.Text.Encodings.Web;

namespace Grand.Web.Framework
{
    public static class HtmlExtensions
    {
        #region Admin area extensions

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this IHtmlHelper<T> helper,
            string name,
            Func<int, HelperResult> localizedTemplate,
            Func<T, HelperResult> standardTemplate,
            bool ignoreIfSeveralStores = false)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
            {
                var localizationSupported = helper.ViewData.Model.Locales.Count > 1;
                if (ignoreIfSeveralStores)
                {
                    var storeService = EngineContextExperimental.Current.Resolve<IStoreService>();
                    if (storeService.GetAllStores().Count >= 2)
                    {
                        localizationSupported = false;
                    }
                }
                if (localizationSupported)
                {
                    var tabStrip = new StringBuilder();
                    tabStrip.AppendLine(string.Format("<div id='{0}'>", name));
                    tabStrip.AppendLine("<ul>");

                    //default tab
                    tabStrip.AppendLine("<li class='k-state-active'>");
                    tabStrip.AppendLine("Standard");
                    tabStrip.AppendLine("</li>");

                    foreach (var locale in helper.ViewData.Model.Locales)
                    {
                        //languages
                        var language = EngineContextExperimental.Current.Resolve<ILanguageService>().GetLanguageById(locale.LanguageId);

                        tabStrip.AppendLine("<li>");
                        //var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                        //var iconUrl = urlHelper.Content("~/Content/images/flags/" + language.FlagImageFileName);
                        //tabStrip.AppendLine(string.Format("<img class='k-image' alt='' src='{0}'>", iconUrl));
                        //tabStrip.AppendLine(WebUtility.HtmlEncode(language.Name));
                        //tabStrip.AppendLine("</li>");
                    }
                    tabStrip.AppendLine("</ul>");



                    //default tab
                    tabStrip.AppendLine("<div>");
                    //tabStrip.AppendLine(standardTemplate(helper.ViewData.Model).ToHtmlString());
                    tabStrip.AppendLine("</div>");

                    for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                    {
                        //languages
                        tabStrip.AppendLine("<div>");
                        //tabStrip.AppendLine(localizedTemplate(i).ToHtmlString());
                        tabStrip.AppendLine("</div>");
                    }
                    tabStrip.AppendLine("</div>");
                    tabStrip.AppendLine("<script>");
                    tabStrip.AppendLine("$(document).ready(function() {");
                    tabStrip.AppendLine(string.Format("$('#{0}').kendoTabStrip(", name));
                    tabStrip.AppendLine("{");
                    tabStrip.AppendLine("animation:  {");
                    tabStrip.AppendLine("open: {");
                    tabStrip.AppendLine("effects: \"fadeIn\"");
                    tabStrip.AppendLine("}");
                    tabStrip.AppendLine("}");
                    tabStrip.AppendLine("});");
                    tabStrip.AppendLine("});");
                    tabStrip.AppendLine("</script>");
                    writer.Write(new HtmlString(tabStrip.ToString()));
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static HtmlString DeleteConfirmation<T>(this IHtmlHelper<T> helper, string buttonsSelector) where T : BaseNopEntityModel
        {
            return DeleteConfirmation(helper, "", buttonsSelector);
        }

        public static HtmlString DeleteConfirmation<T>(this IHtmlHelper<T> helper, string actionName,
            string buttonsSelector) where T : BaseNopEntityModel
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            //var modalId = new HtmlString(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation")
            //    .ToHtmlString();

            //var deleteConfirmationModel = new DeleteConfirmationModel
            //{
            //    Id = helper.ViewData.Model.Id,
            //    //ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
            //    ActionName = actionName,
            //    WindowId = modalId
            //};

            //var window = new StringBuilder();
            //window.AppendLine(string.Format("<div id='{0}' style='display:none;'>", modalId));
            ////window.AppendLine(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString());
            //window.AppendLine("</div>");
            //window.AppendLine("<script>");
            //window.AppendLine("$(document).ready(function() {");
            //window.AppendLine(string.Format("$('#{0}').click(function (e) ", buttonsSelector));
            //window.AppendLine("{");
            //window.AppendLine("e.preventDefault();");
            //window.AppendLine(string.Format("var window = $('#{0}');", modalId));
            //window.AppendLine("if (!window.data('kendoWindow')) {");
            //window.AppendLine("window.kendoWindow({");
            //window.AppendLine("modal: true,");
            //window.AppendLine(string.Format("title: '{0}',", EngineContextExperimental.Current.Resolve<ILocalizationService>().GetResource("Admin.Common.AreYouSure")));
            //window.AppendLine("actions: ['Close']");
            //window.AppendLine("});");
            //window.AppendLine("}");
            //window.AppendLine("window.data('kendoWindow').center().open();");
            //window.AppendLine("});");
            //window.AppendLine("});");
            //window.AppendLine("</script>");

            //return new HtmlString(window.ToString());
            return new HtmlString("");
        }


        public static HtmlString GrandLabelFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null, bool withColumns = true)
        {
            //ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            //string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            //string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            //if (String.IsNullOrEmpty(labelText))
            //{
            //    return HtmlString.Empty;
            //}

            //TagBuilder tag = new TagBuilder("label");
            //tag.MergeAttributes(htmlAttributes);
            ////tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            //if(withColumns)
            //    tag.AddCssClass("control-label col-md-3 col-sm-3");
            ////tag.SetInnerText(labelText);

            //object value;
            //var hintResource = string.Empty;
            //if (metadata.AdditionalValues.TryGetValue("GrandResourceDisplayName", out value))
            //{
            //    var resourceDisplayName = value as GrandResourceDisplayName;
            //    if (resourceDisplayName != null)
            //    {
            //        var langId = EngineContextExperimental.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
            //        hintResource = EngineContextExperimental.Current.Resolve<ILocalizationService>()
            //            .GetResource(resourceDisplayName.ResourceKey + ".Hint", langId);
            //        if (!String.IsNullOrEmpty(hintResource))
            //        {
            //            TagBuilder i = new TagBuilder("i");
            //            i.AddCssClass("help icon-question");
            //            i.Attributes.Add("title", hintResource);
            //            i.Attributes.Add("data-toggle", "tooltip");
            //            i.Attributes.Add("data-placement", "bottom");
            //            //tag.InnerHtml.SetContent(string.Format("{0} {1}", labelText, i.ToString(/*TagRenderMode.Normal*/)));
            //        }
            //    }
            //}
            //return new HtmlString(tag.ToString(/*TagRenderMode.Normal*/));




            return new HtmlString("");

        }

        public static HtmlString GrandEditorFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, bool? renderFormControlClass = null)
        {

            //previous
            //var result = new StringBuilder();
            //object htmlAttributes = null;
            //var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            //if ((!renderFormControlClass.HasValue && metadata.ModelType.Name.Equals("String")) ||
            //    (renderFormControlClass.HasValue && renderFormControlClass.Value))
            //    htmlAttributes = new { @class = "form-control" };
            //result.Append(helper.EditorFor(expression, new { htmlAttributes }));


            //new
            var result = new StringBuilder();
            object htmlAttributes = null;
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);
            if ((!renderFormControlClass.HasValue && metadata.ModelType.Name.Equals("String")) ||
                (renderFormControlClass.HasValue && renderFormControlClass.Value))
                htmlAttributes = new { @class = "form-control" };
            result.Append(helper.EditorFor(expression, new { htmlAttributes }));



            var htmlContentExperimental = helper.EditorFor(expression, new { htmlAttributes });
            using (var writer = new StringWriter(new CultureInfo("en-US")))
            {
                string str = string.Empty;
                htmlContentExperimental.WriteTo(writer, HtmlEncoder.Default);
                writer.Write(str);
                var nNnNnN = writer.ToString();
                return new HtmlString(nNnNnN);
            }



            //var htmlContent = helper.EditorFor(expression);
            //using (var writer = new StringWriter(new CultureInfo("en-US")))
            //{
            //    string str = string.Empty;
            //    htmlContent.WriteTo(writer, HtmlEncoder.Default);
            //    writer.Write(str);
            //    var nNnNnN = writer.ToString();
            //    return new HtmlString(nNnNnN);
            //}
        }

        public static HtmlString GrandDropDownListFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
           Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> itemList,
           object htmlAttributes = null)
        {
            var result = new StringBuilder();

            //var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            //var attrs = IHtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //attrs = AddFormControlClassToHtmlAttributes(attrs);
            //result.Append(helper.DropDownListFor(expression, itemList, attrs));






            return new HtmlString(result.ToString());
        }

        public static HtmlString GrandTextAreaFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
           Expression<Func<TModel, TValue>> expression,
           object htmlAttributes = null)
        {

            //previous
            //var result = new StringBuilder();
            //var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            //var attrs = IHtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //attrs = AddFormControlClassToHtmlAttributes(attrs);
            //result.Append(helper.TextAreaFor(expression, attrs));

            //new
            var result = new StringBuilder();
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, helper.ViewData, helper.MetadataProvider);
            //var attrs = IHtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            dictionary = AddFormControlClassToHtmlAttributes(dictionary);


            result.Append(helper.TextAreaFor(expression, dictionary));



            return new HtmlString(result.ToString());



            //var htmlContentExperimental = helper.EditorFor(expression, new { htmlAttributes });
            //using (var writer = new StringWriter(new CultureInfo("en-US")))
            //{
            //    string str = string.Empty;
            //    htmlContentExperimental.WriteTo(writer, HtmlEncoder.Default);
            //    writer.Write(str);
            //    var nNnNnN = writer.ToString();
            //    return new HtmlString(nNnNnN);
            //}


        }

        public static RouteValueDictionary AddFormControlClassToHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes["class"] == null || string.IsNullOrEmpty(htmlAttributes["class"].ToString()))
                htmlAttributes["class"] = "form-control";
            else
                if (!htmlAttributes["class"].ToString().Contains("form-control"))
                htmlAttributes["class"] += " form-control";

            return htmlAttributes as RouteValueDictionary;
        }

        public static HtmlString OverrideStoreCheckboxFor<TModel, TValue>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue>> forInputExpression,
            string activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.FieldIdFor(forInputExpression));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static HtmlString OverrideStoreCheckboxFor<TModel, TValue1, TValue2>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue1>> forInputExpression1,
            Expression<Func<TModel, TValue2>> forInputExpression2,
            string activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.FieldIdFor(forInputExpression1));
            dataInputIds.Add(helper.FieldIdFor(forInputExpression2));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static HtmlString OverrideStoreCheckboxFor<TModel, TValue1, TValue2, TValue3>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            Expression<Func<TModel, TValue1>> forInputExpression1,
            Expression<Func<TModel, TValue2>> forInputExpression2,
            Expression<Func<TModel, TValue3>> forInputExpression3,
            string activeStoreScopeConfiguration)
        {
            var dataInputIds = new List<string>();
            dataInputIds.Add(helper.FieldIdFor(forInputExpression1));
            dataInputIds.Add(helper.FieldIdFor(forInputExpression2));
            dataInputIds.Add(helper.FieldIdFor(forInputExpression3));
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, null, dataInputIds.ToArray());
        }
        public static HtmlString OverrideStoreCheckboxFor<TModel>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string parentContainer,
            string activeStoreScopeConfiguration)
        {
            return OverrideStoreCheckboxFor(helper, expression, activeStoreScopeConfiguration, parentContainer, null);
        }
        private static HtmlString OverrideStoreCheckboxFor<TModel>(this IHtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string activeStoreScopeConfiguration = "",
            string parentContainer = null,
            params string[] datainputIds)
        {
            if (String.IsNullOrEmpty(parentContainer) && datainputIds == null)
                throw new ArgumentException("Specify at least one selector");

            var result = new StringBuilder();
            if (!String.IsNullOrEmpty(activeStoreScopeConfiguration))
            {
                //render only when a certain store is chosen
                const string cssClass = "multi-store-override-option";
                string dataInputSelector = "";
                if (!String.IsNullOrEmpty(parentContainer))
                {
                    dataInputSelector = "#" + parentContainer + " input, #" + parentContainer + " textarea, #" + parentContainer + " select";
                }
                if (datainputIds != null && datainputIds.Length > 0)
                {
                    dataInputSelector = "#" + String.Join(", #", datainputIds);
                }
                var onClick = string.Format("checkOverriddenStoreValue(this, '{0}')", dataInputSelector);
                result.Append("<label class=\"mt-checkbox\">");
                result.Append(helper.CheckBoxFor(expression, new Dictionary<string, object>
                {
                    { "class", cssClass },
                    { "onclick", onClick },
                    { "data-for-input-selector", dataInputSelector },
                }));
                result.Append("<span></span>");
                result.Append("</label>");
            }
            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Render CSS styles of selected index 
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="currentIndex">Current tab index (where appropriate CSS style should be rendred)</param>
        /// <param name="indexToSelect">Tab index to select</param>
        /// <returns>HtmlString</returns>
        public static HtmlString RenderSelectedTabIndex(this IHtmlHelper helper, int currentIndex, int indexToSelect)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            //ensure it's not negative
            if (indexToSelect < 0)
                indexToSelect = 0;

            //required validation
            if (indexToSelect == currentIndex)
            {
                return new HtmlString(" class='k-state-active'");
            }

            return new HtmlString("");
        }

        #endregion

        #region Common extensions

        public static HtmlString RequiredHint(this IHtmlHelper helper, string additionalText = null)
        {
            // Create tag builder
            var tag = new TagBuilder("span");
            tag.AddCssClass("required");
            var innerText = "*";
            //add additional text if specified
            if (!String.IsNullOrEmpty(additionalText))
                innerText += " " + additionalText;
            //builder.SetInnerText(innerText);
            // Render tag
            //return HtmlString.Create(builder.ToString());



            using (var writer = new StringWriter(new CultureInfo("en-US")))
            {
                string str = string.Empty;
                tag.WriteTo(writer, HtmlEncoder.Default);
                writer.Write(str);
                var nNnNnN = writer.ToString();
                return new HtmlString(nNnNnN);
            }





            //return new HtmlString(builder.ToString());
        }

        public static string FieldNameFor<T, TResult>(this IHtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var test01 = html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));

            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }
        public static string FieldIdFor<T, TResult>(this IHtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            //var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            //because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId


            var idtest = html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            var test02 = idtest.Replace('[', '_').Replace(']', '_');


            //return id.Replace('[', '_').Replace(']', '_');



            return "";
        }

        /// <summary>
        /// Creates a days, months, years drop down list using an HTML select control. 
        /// The parameters represent the value of the "name" attribute on the select control.
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="dayName">"Name" attribute of the day drop down list.</param>
        /// <param name="monthName">"Name" attribute of the month drop down list.</param>
        /// <param name="yearName">"Name" attribute of the year drop down list.</param>
        /// <param name="beginYear">Begin year</param>
        /// <param name="endYear">End year</param>
        /// <param name="selectedDay">Selected day</param>
        /// <param name="selectedMonth">Selected month</param>
        /// <param name="selectedYear">Selected year</param>
        /// <param name="localizeLabels">Localize labels</param>
        /// <param name="htmlAttributes">HTML attributes</param>
        /// <returns></returns>
        public static HtmlString DatePickerDropDowns(this IHtmlHelper html,
            string dayName, string monthName, string yearName,
            int? beginYear = null, int? endYear = null,
            int? selectedDay = null, int? selectedMonth = null, int? selectedYear = null,
            bool localizeLabels = true, object htmlAttributes = null)
        {
            var daysList = new TagBuilder("select");
            var labeldays = new TagBuilder("label");

            var monthsList = new TagBuilder("select");
            var labelmonths = new TagBuilder("label");

            var yearsList = new TagBuilder("select");
            var labelyears = new TagBuilder("label");

            daysList.Attributes.Add("name", dayName);
            daysList.Attributes.Add("id", dayName);
            daysList.Attributes.Add("class", "browser-default");
            labeldays.Attributes.Add("for", dayName);
            labeldays.Attributes.Add("class", "sr-only");

            monthsList.Attributes.Add("name", monthName);
            monthsList.Attributes.Add("id", monthName);
            monthsList.Attributes.Add("class", "browser-default");
            labelmonths.Attributes.Add("for", monthName);
            labelmonths.Attributes.Add("class", "sr-only");

            yearsList.Attributes.Add("name", yearName);
            yearsList.Attributes.Add("id", yearName);
            yearsList.Attributes.Add("class", "browser-default");
            labelyears.Attributes.Add("for", yearName);
            labelyears.Attributes.Add("class", "sr-only");

            var htmlAttributesDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            daysList.MergeAttributes(htmlAttributesDictionary, true);
            monthsList.MergeAttributes(htmlAttributesDictionary, true);
            yearsList.MergeAttributes(htmlAttributesDictionary, true);

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale, monthLocale, yearLocale;
            if (localizeLabels)
            {
                var locService = EngineContextExperimental.Current.Resolve<ILocalizationService>();

                dayLocale = locService.GetResource("Common.Day");
                labeldays.InnerHtml.SetContent(locService.GetResource("Common.Day"));

                monthLocale = locService.GetResource("Common.Month");
                labelmonths.InnerHtml.SetContent(locService.GetResource("Common.Month"));

                yearLocale = locService.GetResource("Common.Year");
                labelyears.InnerHtml.SetContent(locService.GetResource("Common.Year"));
            }
            else
            {
                dayLocale = "Day";
                labeldays.InnerHtml.SetContent("Day");

                monthLocale = "Month";
                labelmonths.InnerHtml.SetContent("Month");

                yearLocale = "Year";
                labelyears.InnerHtml.SetContent("Year");
            }

            days.AppendFormat("<option value='{0}'>{1}</option>", "0", dayLocale);
            for (int i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedDay.HasValue && selectedDay.Value == i) ? " selected=\"selected\"" : null);


            months.AppendFormat("<option value='{0}'>{1}</option>", "0", monthLocale);
            for (int i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                                    i,
                                    (selectedMonth.HasValue && selectedMonth.Value == i) ? " selected=\"selected\"" : null,
                                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }


            years.AppendFormat("<option value='{0}'>{1}</option>", "0", yearLocale);

            if (beginYear == null)
                beginYear = DateTime.UtcNow.Year - 100;
            if (endYear == null)
                endYear = DateTime.UtcNow.Year;

            if (endYear > beginYear)
            {
                for (int i = beginYear.Value; i <= endYear.Value; i++)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);
            }
            else
            {
                for (int i = beginYear.Value; i >= endYear.Value; i--)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);
            }

            daysList.InnerHtml.SetContent(days.ToString());
            monthsList.InnerHtml.SetContent(months.ToString());
            yearsList.InnerHtml.SetContent(years.ToString());


            TagBuilder[] tagBuidlers = new TagBuilder[6] { labeldays, daysList, labelmonths, monthsList, labelyears, yearsList };
            string str323 = string.Empty;
            foreach (var tagBuilder in tagBuidlers)
            {
                using (var writer = new StringWriter(new CultureInfo("en-US")))
                {
                    string str = string.Empty;
                    tagBuilder.WriteTo(writer, HtmlEncoder.Default);
                    writer.WriteLine(str);
                    str323 += writer.ToString();
                    //return new HtmlString(nNnNnN);
                }
            }
            HtmlString htmlString = new HtmlString(str323);




            //var htmlString = string.Concat(labeldays, daysList, labelmonths, monthsList, labelyears, yearsList);

            //using (var writer = new StringWriter(new CultureInfo("en-US")))
            //{
            //    string str = string.Empty;
            //    tag.WriteTo(writer, HtmlEncoder.Default);
            //    writer.Write(str);
            //    var nNnNnN = writer.ToString();
            //    return new HtmlString(nNnNnN);
            //}

            return htmlString;
        }

        public static HtmlString Widget(this IHtmlHelper helper, string widgetZone, object additionalData = null, string area = null)
        {
            //return helper.Action("WidgetsByZone", "Widget", new { widgetZone = widgetZone, additionalData = additionalData, area = area });
            return new HtmlString("");
        }

        /// <summary>
        /// Renders the standard label with a specified suffix added to label text
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="html">HTML helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="htmlAttributes">HTML attributes</param>
        /// <param name="suffix">Suffix</param>
        /// <returns>Label</returns>
        public static HtmlString LabelFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, string suffix)
        {
            //getting field name from expression
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            //dtl, is "FieldName" should be "Field Name"
            //it was previously done by ModelMetadata.FromLambdaExpression(expression, html.ViewData).DisplayName;

            //{<label></label>}
            var tag = new TagBuilder("label");

            //{<label for="FirstName"></label>}
            tag.MergeAttribute("for", TagBuilder.CreateSanitizedId(htmlFieldName, "^"));

            //{<label for="FirstName">First name:</label>}
            var resolvedLabelText = htmlFieldName.Split(new[] { '.' }).Last();
            if (!String.IsNullOrEmpty(suffix))
            {
                resolvedLabelText = String.Concat(resolvedLabelText, suffix);
            }
            tag.InnerHtml.SetHtmlContent(htmlFieldName);

            //{< label class="col-form-label" for="FirstName">First name:</label>}
            var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            tag.MergeAttributes(dictionary, true);

            tag.TagRenderMode = TagRenderMode.Normal;


            //why the ll it dont returns normal string
            var test01 = tag.ToString();




            using (var writer = new StringWriter(new CultureInfo("en-US")))
            {
                string str = string.Empty;
                tag.WriteTo(writer, HtmlEncoder.Default);
                writer.Write(str);
                var nNnNnN = writer.ToString();
                return new HtmlString(nNnNnN);
            }





            //previous
            //string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            //var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            //string resolvedLabelText = metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new[] { '.' }).Last());
            //if (string.IsNullOrEmpty(resolvedLabelText))
            //{
            //    return HtmlString.Empty;
            //}
            //var tag = new TagBuilder("label");

            //tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName)));
            //if (!String.IsNullOrEmpty(suffix))
            //{
            //    resolvedLabelText = String.Concat(resolvedLabelText, suffix);
            //}
            //tag.SetInnerText(resolvedLabelText);

            //var dictionary = IHtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //tag.MergeAttributes(dictionary, true);

            //return new HtmlString(tag.ToString(/*TagRenderMode.Normal*/));
        }

        #endregion
    }
}

