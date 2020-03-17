using CMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class TestPaperDeleteViewModel
    {
        public int TestPaperId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Display(Name = "Test Taken")]
        public bool TestTaken { get; set; }

        [Required]
        [Display(Name = "Type")]
        public TestType TestType { get; set; }

        public string DelimitedQuestionIds { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        public string DelimitedChapterIds { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }

        public int BranchId { get; set; }

        [Display(Name = "Batch")]
        public IEnumerable<SelectListItem> Batches { get; set; }

        public int BatchId { get; set; }

        public string SelectedBatches { get; set; }

        public string SelectedBranches { get; set; }

        public bool Email { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        public string Media { get; set; }

        [Display(Name = "Subject")]
        public string SubjectName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [MaxLength(8, ErrorMessage = "Please select valid time.")]
        [MinLength(8, ErrorMessage = "Please select valid time.")]
        [Display(Name = "Start-Time")]
        public string StartTime { get; set; }

        //[MaxLength(8, ErrorMessage = "Please select valid time.")]
        //[MinLength(8, ErrorMessage = "Please select valid time.")]
        //[Display(Name = "End-Time")]
        //public string EndTime { get; set; }

        [Display(Name = "Time Duration")]
        public int TimeDuration { get; set; }
    }
}