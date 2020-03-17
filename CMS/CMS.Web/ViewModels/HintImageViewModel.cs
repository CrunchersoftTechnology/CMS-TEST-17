using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Web.ViewModels
{
    public class HintImageViewModel
    {
        public int QuestionId { get; set; }
        public HttpPostedFileBase HintImage { get; set; }
    }
}