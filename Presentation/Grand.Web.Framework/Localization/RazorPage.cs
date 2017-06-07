﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Grand.Core.Infrastructure;
using Grand.Services.Localization;
//using Orchard.DisplayManagement.Layout;
//using Orchard.DisplayManagement.Shapes;
//using Orchard.DisplayManagement.Title;
using Grand.Web.Framework.Localization;
using System.Diagnostics;

namespace Grand.Web.Framework.Localization
{
    public abstract class RazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        //private dynamic _displayHelper;
        ////private IShapeFactory _shapeFactory;

        //private void EnsureDisplayHelper()
        //{
        //    if (_displayHelper == null)
        //    {
        //        //IDisplayHelperFactory _factory = ViewContext.HttpContext.RequestServices.GetService<IDisplayHelperFactory>();
        //        //_displayHelper = _factory.CreateHelper(ViewContext);
        //    }
        //}

        ////private void EnsureShapeFactory()
        ////{
        ////    if (_shapeFactory == null)
        ////    {
        ////        _shapeFactory = ViewContext.HttpContext.RequestServices.GetService<IShapeFactory>();
        ////    }
        ////}

        ///// <summary>
        ///// Gets a dynamic shape factory to create new shapes.
        ///// </summary>
        ///// <example>
        ///// Usage:
        ///// <code>
        ///// New.MyShape()
        ///// New.MyShape(A: 1, B: "Some text")
        ///// New.MyShape().A(1).B("Some text")
        ///// </code>
        ///// </example>
        ////public dynamic New
        ////{
        ////    get
        ////    {
        ////        return Factory;
        ////    }
        ////}

        ///// <summary>
        ///// Gets an <see cref="IShapeFactory"/> to create new shapes.
        ///// </summary>
        ////public IShapeFactory Factory
        ////{
        ////    get
        ////    {
        ////        EnsureShapeFactory();
        ////        return _shapeFactory;
        ////    }
        ////}

        ///// <summary>
        ///// Renders a shape.
        ///// </summary>
        ///// <param name="shape">The shape.</param>
        //public Task<IHtmlContent> DisplayAsync(dynamic shape)
        //{
        //    EnsureDisplayHelper();
        //    return (Task<IHtmlContent>)_displayHelper(shape);
        //}

        //private dynamic _themeLayout;
        //public dynamic ThemeLayout
        //{
        //    get
        //    {
        //        if (_themeLayout == null)
        //        {
        //            var layoutAccessor = ViewContext.HttpContext.RequestServices.GetService<>();

        //            if (layoutAccessor == null)
        //            {
        //                throw new InvalidOperationException("Could not find a valid layout accessor");
        //            }

        //            _themeLayout = layoutAccessor.GetLayout();
        //        }

        //        return _themeLayout;
        //    }

        //    set
        //    {
        //        _themeLayout = value;
        //    }
        //}

        //private IPageTitleBuilder _pageTitleBuilder;
        //public IPageTitleBuilder Title
        //{
        //    get
        //    {
        //        if (_pageTitleBuilder == null)
        //        {
        //            _pageTitleBuilder = ViewContext.HttpContext.RequestServices.GetRequiredService<IPageTitleBuilder>();
        //        }

        //        return _pageTitleBuilder;
        //    }
        //}

        private IViewLocalizer _t;



        //doesnt work well 
        //public IViewLocalizer T
        //{
        //    get
        //    {
        //        if (_t == null)
        //        {
        //            var test01 = _t.GetAllStrings();
        //            var test02 = _t.GetHtml("raz.dwa.trzy");
        //            var test03 = _t.ToString();
        //            var test04 = _t.WithCulture(new System.Globalization.CultureInfo("en-us"));

        //            var _localizationService = EngineContextExperimental.Current.Resolve<ILocalizationService>();
        //            var result = _localizationService.GetResource(/*format*/_t.ToString());

        //            _t = ViewContext.HttpContext.RequestServices.GetRequiredService<IViewLocalizer>();
        //            ((IViewContextAware)_t).Contextualize(this.ViewContext);
        //        }

        //        return _t;
        //    }
        //}





        //[Inject]
        private Localizer _localizer;
        public Localizer T
        {
            get
            {

                if (_localizer == null)
                {
                    //null localizer
                    //_localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));

                    //default localizer
                    _localizer = (format, args) =>
                    {

                        Debug.WriteLine("locale\t\t" + format);

                        var _localizationService = EngineContextExperimental.Current.Resolve<ILocalizationService>();
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

                if (_localizer == null)
                {

                }

                if (_t == null)
                {
                    var test01 = _t.GetAllStrings();
                    var test02 = _t.GetHtml("raz.dwa.trzy");
                    var test03 = _t.ToString();
                    var test04 = _t.WithCulture(new System.Globalization.CultureInfo("en-us"));

                    var _localizationService = EngineContextExperimental.Current.Resolve<ILocalizationService>();
                    var result = _localizationService.GetResource(/*format*/_t.ToString());

                    _t = ViewContext.HttpContext.RequestServices.GetRequiredService<IViewLocalizer>();
                    ((IViewContextAware)_t).Contextualize(this.ViewContext);
                }
                return null;

                //return _t;
            }
        }

































        ///// <summary>
        ///// Adds a segment to the title and returns all segments.
        ///// </summary>
        ///// <param name="segment">The segment to add to the title.</param>
        ///// <param name="position">Optional. The position of the segment in the title.</param>
        ///// <param name="separator">The html string that should separate all segments.</param>
        ///// <returns>And <see cref="IHtmlContent"/> instance representing the full title.</returns>
        ////public IHtmlContent RenderTitleSegments(IHtmlContent segment, string position = "0", IHtmlContent separator = null)
        ////{
        ////    Title.AddSegment(segment, position);
        ////    return Title.GenerateTitle(separator);
        ////}

        ///// <summary>
        ///// Adds some segments to the title and returns all segments.
        ///// </summary>
        ///// <param name="segments">The segments to add to the title.</param>
        ///// <param name="position">Optional. The position of the segments in the title.</param>
        ///// <returns>And <see cref="IHtmlContent"/> instance representing the full title.</returns>
        ////public IHtmlContent RenderTitleSegments(IEnumerable<IHtmlContent> segments, string position = "0", IHtmlContent separator = null)
        ////{
        ////    Title.AddSegments(segments, position);
        ////    return Title.GenerateTitle(separator);
        ////}

        ///// <summary>
        ///// Renders the content zone of the layout.
        ///// </summary>
        //protected IHtmlContent RenderLayoutBody()
        //{
        //    var result = base.RenderBody();
        //    return result;
        //}

        ///// <summary>
        ///// Creates a <see cref="TagBuilder"/> to render a shape.
        ///// </summary>
        ///// <param name="shape">The shape.</param>
        ///// <returns>A new <see cref="TagBuilder"/>.</returns>
        ////protected TagBuilder Tag(dynamic shape)
        ////{
        ////    return Shape.GetTagBuilder(shape);
        ////}

        ////protected TagBuilder Tag(dynamic shape, string tag)
        ////{
        ////    return Shape.GetTagBuilder(shape, tag);
        ////}

        //protected Task<IHtmlContent> RenderBodyAsync()
        //{
        //    return DisplayAsync(ThemeLayout.Content);
        //}

        ///// <summary>
        ///// Renders a zone from the layout.
        ///// </summary>
        ///// <param name="name">The name of the zone to render.</param>
        ///// <param name="required">Whether the zone is required or not.</param>
        //public new Task<IHtmlContent> RenderSectionAsync(string name, bool required)
        //{
        //    if (name == null)
        //    {
        //        throw new ArgumentNullException(nameof(name));
        //    }

        //    var zone = ThemeLayout[name];

        //    if (required && zone != null && !zone.Items.Any())
        //    {
        //        throw new InvalidOperationException("Zone not found: " + name);
        //    }

        //    return DisplayAsync(zone);
        //}

        //public object OrDefault(object text, object other)
        //{
        //    if (text == null || Convert.ToString(text) == "")
        //    {
        //        return other;
        //    }

        //    return text;
        //}

        ///// <summary>
        ///// Returns the full path of the current request.
        ///// </summary>
        //public string FullRequestPath => Context.Request.PathBase + Context.Request.Path + Context.Request.QueryString;
    }

    //public abstract class RazorPage : RazorPage<dynamic>
    //{
    //}
}
