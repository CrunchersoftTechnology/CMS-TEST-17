using System;

namespace CMS.Common.GridModels
{
    public class UploadTextbooksGridModel
    {
        public int UploadTextbooksId { get; set; }

        public string BoardName { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        public string Title { get; set; }

        public DateTime UploadDate { get; set; }

        public string FileName { get; set; }

        public string LogoName { get; set; }

        public bool IsVisible { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
