using System.Collections.Generic;

namespace CMS.Web.Models
{
    public class SendNotification
    {
        public string Message { get; set; }
        public string AppIds { get; set; }
        public List<string> PlayerIds { get; set; }
        public string RestApiKey { get; set; }
    }

    public class SendNotificationByPlayerId
    {
        public string Message { get; set; }
        public string AppIds { get; set; }
        public string PlayerIds { get; set; }
        public string RestApiKey { get; set; }
    }
}