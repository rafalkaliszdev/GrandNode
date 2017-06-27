using System.Collections.Generic;
/*using System.Web.Mvc;*/using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation.Attributes;
//using Grand.Admin.Validators.Customers;
using Grand.Web.Framework;
using Grand.Web.Framework.Localization;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Areas.Admin.Models.Customers
{
    //[Validator(typeof(CustomerAttributeValueValidator))]
    public partial class CustomerAttributeValueModel : BaseNopEntityModel, ILocalizedModel<CustomerAttributeValueLocalizedModel>
    {
        public CustomerAttributeValueModel()
        {
            Locales = new List<CustomerAttributeValueLocalizedModel>();
        }

        public string CustomerAttributeId { get; set; }

        [GrandResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }

        [GrandResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [GrandResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<CustomerAttributeValueLocalizedModel> Locales { get; set; }

    }

    public partial class CustomerAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        /*[AllowHtml]*/
        public string Name { get; set; }
    }
}