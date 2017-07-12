﻿
using Grand.Web.Framework;
using Grand.Web.Framework.Mvc;

namespace Grand.Web.Models.Blogs
{
    public partial class AddBlogCommentModel : BaseNopEntityModel
    {
        [GrandResourceDisplayName("Blog.Comments.CommentText")]
        /*[AllowHtml]*/
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}