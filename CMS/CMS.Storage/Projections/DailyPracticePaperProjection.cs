using System;
using System.Web;

namespace CMS.Domain.Storage.Projections
{
    public class DailyPracticePaperProjection
    {
        public int DailyPracticePaperId { get; set; }

        public string Description { get; set; }

        public string SelectedBranches { get; set; }

        public string SelectedClasses { get; set; }

        public string SelectedBatches { get; set; }

        public string FileName { get; set; }

        public HttpPostedFileBase FilePath { get; set; }

        public DateTime CreatedOn { get; set; }

        public string AttachmentDescription { get; set; }

        public DateTime DailyPracticePaperDate { get; set; }
    }
}
