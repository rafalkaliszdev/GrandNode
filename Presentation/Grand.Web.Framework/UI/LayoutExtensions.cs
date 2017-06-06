/*using System.Web.Mvc;*/
using Grand.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Grand.Web.Framework.Themes;
using Autofac;

namespace Grand.Web.Framework.UI
{
    public static class LayoutExtensions
    {

        /// <summary>
        /// Add title element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Title part</param>
        public static void AddTitleParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddTitleParts(part);
        }
        /// <summary>
        /// Append title element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Title part</param>
        public static void AppendTitleParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendTitleParts(part);
        }
        /// <summary>
        /// Generate all title parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="addDefaultTitle">A value indicating whether to insert a default title</param>
        /// <param name="part">Title part</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopTitle(this IHtmlHelper html, bool addDefaultTitle, string part = "")
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            html.AppendTitleParts(part);


            var test01 = html.Encode(pageHeadBuilder.GenerateTitle(addDefaultTitle));

            return new HtmlString(html.Encode(pageHeadBuilder.GenerateTitle(addDefaultTitle)));
        }


        /// <summary>
        /// Add meta description element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Meta description part</param>
        public static void AddMetaDescriptionParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddMetaDescriptionParts(part);
        }
        /// <summary>
        /// Append meta description element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Meta description part</param>
        public static void AppendMetaDescriptionParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendMetaDescriptionParts(part);
        }
        /// <summary>
        /// Generate all description parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Meta description part</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopMetaDescription(this IHtmlHelper html, string part = "")
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            html.AppendMetaDescriptionParts(part);

            var test01 = html.Encode(pageHeadBuilder.GenerateMetaDescription());

            return  new HtmlString(html.Encode(pageHeadBuilder.GenerateMetaDescription()));
        }


        /// <summary>
        /// Add meta keyword element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Meta keyword part</param>
        public static void AddMetaKeywordParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddMetaKeywordParts(part);
        }
        /// <summary>
        /// Append meta keyword element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Meta keyword part</param>
        public static void AppendMetaKeywordParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendMetaKeywordParts(part);
        }
        /// <summary>
        /// Generate all keyword parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Meta keyword part</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopMetaKeywords(this IHtmlHelper html, string part = "")
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            html.AppendMetaKeywordParts(part);

            var test01 = html.Encode(pageHeadBuilder.GenerateMetaKeywords());

            return new HtmlString(html.Encode(pageHeadBuilder.GenerateMetaKeywords()));

        }


        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Script part</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AddScriptParts(this IHtmlHelper html, string part, bool excludeFromBundle = false, bool isAsync = false)
        {
            AddScriptParts(html, ResourceLocation.Head, part, excludeFromBundle, isAsync);
        }
        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="part">Script part</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AddScriptParts(this IHtmlHelper html, ResourceLocation location, string part, bool excludeFromBundle = false, bool isAsync = false)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddScriptParts(location, part, excludeFromBundle, isAsync);
        }
        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Script part</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AppendScriptParts(this IHtmlHelper html, string part, bool excludeFromBundle = false, bool isAsync = false)
        {
            AppendScriptParts(html, ResourceLocation.Head, part, excludeFromBundle, isAsync);
        }
        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="part">Script part</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AppendScriptParts(this IHtmlHelper html, ResourceLocation location, string part, bool excludeFromBundle = false, bool isAsync = false)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendScriptParts(location, part, excludeFromBundle, isAsync);
        }
        /// <summary>
        /// Generate all script parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="IUrlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="bundleFiles">A value indicating whether to bundle script elements</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopScripts(this IHtmlHelper html, IUrlHelper IUrlHelper,
            ResourceLocation location, bool? bundleFiles = null)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();

            var test01 = pageHeadBuilder.GenerateScripts(IUrlHelper, location, bundleFiles);

            return new HtmlString(pageHeadBuilder.GenerateScripts(IUrlHelper, location, bundleFiles));
        }



        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">CSS part</param>
        public static void AddCssFileParts(this IHtmlHelper html, string part, bool excludeFromBundle = false)
        {
            AddCssFileParts(html, ResourceLocation.Head, part, excludeFromBundle);
        }
        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="part">CSS part</param>
        public static void AddCssFileParts(this IHtmlHelper html, ResourceLocation location, string part, bool excludeFromBundle = false)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();


            pageHeadBuilder.AddCssFileParts(location, part, excludeFromBundle);
        }
        /// <summary>
        /// Append CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">CSS part</param>
        public static void AppendCssFileParts(this IHtmlHelper html, string part, bool excludeFromBundle = false)
        {
            AppendCssFileParts(html, ResourceLocation.Head, part, excludeFromBundle);
        }
        /// <summary>
        /// Append CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="part">CSS part</param>
        public static void AppendCssFileParts(this IHtmlHelper html, ResourceLocation location, string part, bool excludeFromBundle = false)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendCssFileParts(location, part, excludeFromBundle);
        }
        /// <summary>
        /// Generate all CSS parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="IUrlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="bundleFiles">A value indicating whether to bundle script elements</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopCssFiles(this IHtmlHelper html, IUrlHelper IUrlHelper,
            ResourceLocation location, bool? bundleFiles = null)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
    


            var test01 = html.Encode(pageHeadBuilder.GenerateCssFiles(IUrlHelper, location, bundleFiles));



            //previous
            //return new HtmlString(html.Encode(pageHeadBuilder.GenerateCssFiles(IUrlHelper, location, bundleFiles)));
            //new
            return new HtmlString(pageHeadBuilder.GenerateCssFiles(IUrlHelper, location, bundleFiles));



        }


        /// <summary>
        /// Add canonical URL element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Canonical URL part</param>
        public static void AddCanonicalUrlParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddCanonicalUrlParts(part);
        }
        /// <summary>
        /// Append canonical URL element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Canonical URL part</param>
        public static void AppendCanonicalUrlParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendCanonicalUrlParts(part);
        }
        /// <summary>
        /// Generate all canonical URL parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Canonical URL part</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopCanonicalUrls(this IHtmlHelper html, string part = "")
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            html.AppendCanonicalUrlParts(part);
 

            var test01 = html.Encode(pageHeadBuilder.GenerateCanonicalUrls());

            return new HtmlString(html.Encode(pageHeadBuilder.GenerateCanonicalUrls()));



        }

        /// <summary>
        /// Add any custom element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">The entire element. For example, <![CDATA[<meta name="msvalidate.01" content="123121231231313123123" />]]></param>
        public static void AddHeadCustomParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddHeadCustomParts(part);
        }
        /// <summary>
        /// Append any custom element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">The entire element. For example, <![CDATA[<meta name="msvalidate.01" content="123121231231313123123" />]]></param>
        public static void AppendHeadCustomParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendHeadCustomParts(part);
        }
        /// <summary>
        /// Generate all custom elements
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <returns>Generated string</returns>
        public static HtmlString NopHeadCustom(this IHtmlHelper html)
        {
            var pageHeadBuilder = EngineContextExperimental.Current.Resolve<IPageHeadBuilder>();

            var test01 = html.Encode(pageHeadBuilder.GenerateHeadCustom());

            return new HtmlString(html.Encode(pageHeadBuilder.GenerateHeadCustom()));
        }
    }
}
