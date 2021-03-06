﻿using System;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Framework.Mvc
{
    public class NullJsonResult : JsonResult
    {
        public NullJsonResult(object value) : base(value)
        {
        }

        //public override void ExecuteResult(ControllerContext context)
        //{
        //    if (context == null)
        //        throw new ArgumentNullException("context");

        //    //we do it as described here - http://stackoverflow.com/questions/15939944/jquery-post-json-fails-when-returning-null-from-asp-net-mvc

        //    var response = context.HttpContext.Response;
        //    response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
        //    if (ContentEncoding != null)
        //        response.ContentEncoding = ContentEncoding;

        //    this.Data = null;

        //    //If you need special handling, you can call another form of SerializeObject below
        //    var serializedObject = JsonConvert.SerializeObject(Data, Formatting.Indented);
        //    response.Write(serializedObject);
        //}
    }
}
