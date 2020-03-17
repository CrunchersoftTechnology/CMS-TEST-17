using System;

namespace CMS.Common.GridModels
{
    public class DailyPracticePaperGridModel
    {
        public int DailyPracticePaperId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FileName { get; set; }

        public string Action { get; set; }

        public string SelectedBranches { get; set; }

        public DateTime DailyPracticePaperDate { get; set; }
    }
}
