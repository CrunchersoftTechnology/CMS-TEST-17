using System.Collections.Generic;

namespace CMS.Web.Models
{
    public class MailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBranchAdmin { get; set; }
        public bool IsClientAdmin { get; set; }
        public string Emailid { get; set; }
        public string Password { get; set; }
        public List<string> AttachmentPaths { get; set; }

        public MailModel()
        {
            AttachmentPaths = new List<string>();
        }
    }
}
