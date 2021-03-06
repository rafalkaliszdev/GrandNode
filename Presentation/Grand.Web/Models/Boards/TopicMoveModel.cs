﻿using System.Collections.Generic;

using Grand.Web.Framework.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grand.Web.Models.Boards
{
    public partial class TopicMoveModel : BaseNopEntityModel
    {
        public TopicMoveModel()
        {
            ForumList = new List<SelectListItem>();
        }

        public string ForumSelected { get; set; }
        public string TopicSeName { get; set; }

        public IEnumerable<SelectListItem> ForumList { get; set; }
    }
}