﻿
using FluentValidation.Attributes;
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;
using Grand.Web.Validators.Customer;

namespace Grand.Web.Models.Customer
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public partial class PasswordRecoveryModel : BaseNopModel
    {
        /*[AllowHtml]*/
        [GrandResourceDisplayName("Account.PasswordRecovery.Email")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}