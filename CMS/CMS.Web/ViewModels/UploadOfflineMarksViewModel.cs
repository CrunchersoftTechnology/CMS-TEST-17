using CMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class UploadOfflineMarksViewModel
    {
        public int OfflineTestStudentMarksId { get; set; }

        [Required(ErrorMessage = "The Offline Test Paper is required.")]
        public int OfflineTestPaperId { get; set; }

        [Display(Name = "Paper")]
        public IEnumerable<SelectListItem> Papers { get; set; }

      //  [Required(ErrorMessage = "The Student Name field is required.")]
        public string UserId { get; set; }

        //[Display(Name = "Student")]
        //public IEnumerable<SelectListItem> Students { get; set; }

        public IEnumerable<SelectListItem> Branches { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; }

        public IEnumerable<SelectListItem> Classes { get; set; }

        public IEnumerable<SelectListItem> Batches { get; set; }

        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Client")]
        public int? ClientId { get; set; }


        [Display(Name = "Batch")]
        public int? BatchId { get; set; }

        [Display(Name = "Class")]
        public int? ClassId { get; set; }

        [Display(Name = "Subject")]
        public int? SubjectId { get; set; }

        public string CurrentUserRole { get; set; }

        public string BranchName { get; set; }

        [Display(Name = "Branch")]
        public string SelectedBranches { get; set; }

        public string ClientName { get; set; }

      


        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        [Display(Name = "Batch")]
        public string SelectedBatches { get; set; }

        [Display(Name = "Batch")]
        public string SelectedBatchesName { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<SelectListItem> Subjects { get; set; }

        public bool Email { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        [Display(Name = "Branch")]
        public string SelectedBranchesName { get; set; }

        [Required(ErrorMessage = "Atleast one student must be available.")]
        [Display(Name = "Student List")]
        public string StudentOfflineMarks { get; set; }

        public decimal Percentage { get; set; }

        public int ObtainedMarks { get; set; }

        public string StudentEmail { get; set; }

        public string StudentContact { get; set; }

        [Display(Name = "Total Marks")]
        public int TotalMarks { get; set; }

        public string Title { get; set; }

        public bool IsPresent { get; set; }
    }
}