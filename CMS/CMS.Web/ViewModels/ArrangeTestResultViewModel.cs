using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Web.ViewModels
{
    public class ArrangeTestResultViewModel
    {
        public int ArrangeTestResultId { get; set; }

        public string UserId { get; set; }

        public int TestPaperId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Test Date")]
        public DateTime TestDate { get; set; }

        [Display(Name = "Time Duration")]
        public int TimeDuration { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        public string Questions { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Display(Name = "Test Paper Title")]
        public string TestPaperTitle { get; set; }

        [Display(Name = "Obtained Marks")]
        public int ObtainedMarks { get; set; }

        [Display(Name = "OutOf Marks")]
        public int OutOfMarks { get; set; }
    }
}