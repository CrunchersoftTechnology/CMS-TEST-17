using System;
using System.Web;

namespace CMS.Domain.Storage.Projections
{
    public class NewsProjection
    {
        public int UploadNewsId { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public HttpPostedFileBase FilePath { get; set; }

        public bool IsVisible { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
