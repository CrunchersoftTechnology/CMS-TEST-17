using System;
using System.Web;

namespace CMS.Domain.Storage.Projections
{
    public class PDFUploadProjection
    {
        public int ClassId { get; set; }

        public int PDFUploadId { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public HttpPostedFileBase FilePath { get; set; }

        public bool IsVisible { get; set; }
        
        public string ClassName { get; set; }

        public int PDFCategoryId { get; set; }

        public string PDFCategoryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsSend { get; set; }
    }
}
