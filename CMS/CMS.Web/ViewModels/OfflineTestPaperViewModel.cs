using CMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace CMS.Web.ViewModels
{
    public class OfflineTestPaperViewModel
    {
        public int OfflineTestPaperId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field Title must be a minimum length of '3' and maximum length of '100'.")]
        [MinLength(3, ErrorMessage = "The field Title must be a minimum length of '3' and maximum length of '100'.")]
        public string Title { get; set; }

        [Display(Name = "Branch")]
        public string SelectedBranches { get; set; }

        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Display(Name = "Batch")]
        public string SelectedBatches { get; set; }

        [Display(Name = "Total Marks")]
        //[Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Marks must be numeric.")]
        [Required]
        [Range(1, 999, ErrorMessage = "Total Marks length upto 3 digit.")]
        public int TotalMarks { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Test Date")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TestDate { get; set; }

        [MaxLength(8)]
        [MinLength(8)]
        [Display(Name = "In-Time")]
        public string TestInTime { get; set; }

        [MaxLength(8)]
        [MinLength(8)]
        [Display(Name = "Out-Time")]
        public string TestOutTime { get; set; }

        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Client")]
        public int? ClientId { get; set; }

        [Display(Name = "Batch")]
        public int? BatchId { get; set; }

        public IEnumerable<SelectListItem> Branches { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; }

        public IEnumerable<SelectListItem> Classes { get; set; }

        public IEnumerable<SelectListItem> Batches { get; set; }

        public IEnumerable<SelectListItem> Subjects { get; set; }

        public string CurrentUserRole { get; set; }

        public string BranchName { get; set; }


        public string ClientName { get; set; }

        public string Media { get; set; }

        public bool Email { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        [Display(Name = "Branch")]
        public string SelectedBranchesName { get; set; }

        [Display(Name = "Batch")]
        public string SelectedBatchesName { get; set; }
    }
}