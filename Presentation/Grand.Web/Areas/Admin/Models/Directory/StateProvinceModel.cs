﻿using System.Collections.Generic;
/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Directory;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Directory
{
    //[Validator(typeof(StateProvinceValidator))]
    public partial class StateProvinceModel : BaseNopEntityModel, ILocalizedModel<StateProvinceLocalizedModel>
    {
        public StateProvinceModel()
        {
            Locales = new List<StateProvinceLocalizedModel>();
        }
        public string CountryId { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Countries.States.Fields.Abbreviation")]
        /*[AllowHtml]*/
        public string Abbreviation { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }

        [GrandResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<StateProvinceLocalizedModel> Locales { get; set; }
    }

    public partial class StateProvinceLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }
        
        [GrandResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }
    }
}