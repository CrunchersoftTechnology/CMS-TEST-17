using CMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class OfflineTestStudentMarksViewModel
    {
        public int OfflineTestStudentMarksId { get; set; }

        [Required(ErrorMessage = "The Offline Test Paper is required.")]
        public int OfflineTestPaperId { get; set; }

        [Display(Name = "Paper")]
        public IEnumerable<SelectListItem> Papers { get; set; }

        [Display(Name = "Paper")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Student Name field is required.")]
        public string UserId { get; set; }

        [Display(Name = "Student")]
        public IEnumerable<SelectListItem> Students { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Display(Name = "Student Marks")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Marks must be numeric.")]
        public int MarksObtained { get; set; }

        [Display(Name = "Total Marks")]
        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Marks must be numeric.")]
        public int TotalMarks{ get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Display(Name = "Batch Name")]
        public string BatchName { get; set; }

        public string StudBatch { get; set; }

        public int ClassId { get; set; }

        public string SelectedBatches { get; set; }

        public string SelectedBranches { get; set; }

        public bool Email { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        public string StudentEmail { get; set; }

        public string StudentContact { get; set; }
    }
}