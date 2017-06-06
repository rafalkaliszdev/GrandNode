﻿using Grand.Web.Framework.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace Grand.Web.Models.Newsletter
{
    public partial class NewsletterCategoryModel : BaseNopModel
    {
        public NewsletterCategoryModel()
        {
            NewsletterCategories = new List<NewsletterSimpleCategory>();
        }
        public string NewsletterEmailId { get; set; }
        public IList<NewsletterSimpleCategory> NewsletterCategories { get; set; }
    }
    public class NewsletterSimpleCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
}