﻿/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Polls;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Grand.Web.Framework.Localization;
using System.Collections.Generic;

namespace Grand.Web.Areas.Admin.Models.Polls
{
    //[Validator(typeof(PollAnswerValidator))]
    public partial class PollAnswerModel : BaseNopEntityModel, ILocalizedModel<PollAnswerLocalizedModel>
    {
        public PollAnswerModel()
        {
            Locales = new List<PollAnswerLocalizedModel>();
        }

        public string PollId { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.NumberOfVotes")]
        public int NumberOfVotes { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<PollAnswerLocalizedModel> Locales { get; set; }


    }

    public partial class PollAnswerLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.ContentManagement.Polls.Answers.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }
    }


}