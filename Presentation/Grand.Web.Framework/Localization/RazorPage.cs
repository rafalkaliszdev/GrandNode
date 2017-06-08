using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Infrastructure;
using Grand.Services.Localization;
using Grand.Web.Framework.Themes;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Grand.Web.Framework.Localization
{
    public abstract class RazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        private ILocalizationService _localizationService;
        private Localizer _localizer;
        private IWorkContext _workContext;

        /// <summary>
        /// Get a localized resources
        /// </summary>
        /// 
        public Localizer T
        {
            get
            {

                //InitHelpers();

                if (_localizer == null)
                {
                    //null localizer
                    //_localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));

                    //default localizer
                    _localizer = (format, args) =>
                    {
                        //workaround
                        if (_localizationService == null)
                            _localizationService = EngineContextExperimental.Current.Resolve<ILocalizationService>();

                        var resFormat = _localizationService.GetResource(format);
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return
                            new LocalizedString((args == null || args.Length == 0)
                                                    ? resFormat
                                                    : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }

        public IWorkContext WorkContext
        {
            get
            {
                return _workContext;
            }
        }

        public /*override*/ void InitHelpers()
        {
            //nothing to override in RazorPage..
            //base.InitHelpers();

            //if (DataSettingsHelper.DatabaseIsInstalled())
            //{
            //    _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            //    _workContext = EngineContext.Current.Resolve<IWorkContext>();
            //}
        }

        public /*override*/ string Layout
        {
            get
            {
                var layout = base.Layout;

                //if (!string.IsNullOrEmpty(layout))
                //{
                //    var filename = Path.GetFileNameWithoutExtension(layout);
                //    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

                //    if (viewResult.View != null && viewResult.View is RazorView)
                //    {
                //        layout = (viewResult.View as RazorView).ViewPath;
                //    }
                //}

                return layout;
            }
            set
            {
                base.Layout = value;
            }
        }

        /// <summary>
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns></returns>
        public bool ShouldUseRtlTheme()
        {
            var supportRtl = _workContext.WorkingLanguage.Rtl;
            if (supportRtl)
            {
                //ensure that the active theme also supports it
                var themeProvider = EngineContext.Current.Resolve<IThemeProvider>();
                var themeContext = EngineContext.Current.Resolve<IThemeContext>();
                supportRtl = themeProvider.GetThemeConfiguration(themeContext.WorkingThemeName).SupportRtl;
            }
            return supportRtl;
        }

        public string WorkingLanguage()
        {
            return _workContext.WorkingLanguage.UniqueSeoCode;
        }


        /// <summary>
        /// Gets a selected tab index (used in admin area to store selected tab index)
        /// </summary>
        /// <returns>Index</returns>
        public int GetSelectedTabIndex()
        {
            //keep this method synchornized with
            //"SetSelectedTabIndex" method of \Administration\Controllers\BaseNopController.cs
            int index = 0;
            string dataKey = "Grand.selected-tab-index";
            if (ViewData[dataKey] is int)
            {
                index = (int)ViewData[dataKey];
            }
            if (TempData[dataKey] is int)
            {
                index = (int)TempData[dataKey];
            }

            //ensure it's not negative
            if (index < 0)
                index = 0;

            return index;
        }
    }

    public abstract class RazorPage : RazorPage<dynamic>
    {
    }
}
