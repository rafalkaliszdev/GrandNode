﻿/*using System.Web.Mvc;*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Grand.Web.Framework.UI
{
    public partial interface IPageHeadBuilder
    {
        void AddTitleParts(string part);
        void AppendTitleParts(string part);
        string GenerateTitle(bool addDefaultTitle);

        void AddMetaDescriptionParts(string part);
        void AppendMetaDescriptionParts(string part);
        string GenerateMetaDescription();

        void AddMetaKeywordParts(string part);
        void AppendMetaKeywordParts(string part);
        string GenerateMetaKeywords();

        void AddScriptParts(ResourceLocation location, string part, bool excludeFromBundle, bool isAync);
        void AppendScriptParts(ResourceLocation location, string part, bool excludeFromBundle, bool isAync);
        string GenerateScripts(IUrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null);

        void AddCssFileParts(ResourceLocation location, string part, bool excludeFromBundle = false);
        void AppendCssFileParts(ResourceLocation location, string part, bool excludeFromBundle = false);
        string GenerateCssFiles(IUrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null);


        void AddCanonicalUrlParts(string part);
        void AppendCanonicalUrlParts(string part);
        string GenerateCanonicalUrls();

        void AddHeadCustomParts(string part);
        void AppendHeadCustomParts(string part);
        string GenerateHeadCustom();
        void AddEditPageUrl(string url);
        string GetEditPageUrl();
    }

}
