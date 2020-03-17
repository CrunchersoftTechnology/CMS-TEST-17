using System;

namespace CMS.Common.GridModels
{
    public class AnnouncementGridModel
    {
        public int AnnouncementId { get; set; }

        public string AnnouncementDetails { get; set; }

        public string Url { get; set; }

        public bool IsVisible { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        [Exclude]
        public string Action { get; set; }
    }
}
