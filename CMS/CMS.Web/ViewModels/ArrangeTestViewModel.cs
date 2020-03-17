using CMS.Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Web.ViewModels
{
    public class ArrangeTestViewModel
    {
        public int ArrengeTestId { get; set; }

        public int TestPaperId { get; set; }

        [Display(Name = "Class")]
        public string SelectedClass { get; set; }

        [Display(Name = "Branches")]
        public string SelectedBranches { get; set; }

        [Display(Name = "Batches")]
        public string SelectedBatches { get; set; }

        [ForeignKey("TestPaperId")]
        public virtual TestPaper TestPapers { get; set; }

        [Display(Name = "Student Count")]
        public int StudentCount { get; set; }

        public string Media { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Type")]
        public string TestType { get; set; }

        [Display(Name = "Subject")]
        public string SubjectName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Display(Name = "Time Duration")]
        public int TimeDuration { get; set; }
    }
}