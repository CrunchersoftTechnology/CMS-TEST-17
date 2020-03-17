using System;

namespace CMS.Domain.Storage.Projections
{
    public class AnnouncementProjection
    {
        public int AnnouncementId { get; set; }
        public string AnnouncementDetails { get; set; }
        public string Url { get; set; }
        public bool IsVisible { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
