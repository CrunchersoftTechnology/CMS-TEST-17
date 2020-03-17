using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using CMS.Common;

namespace CMS.Web.ViewModels
{
    public class DailyPracticePaperViewModel
    {
        public int DailyPracticePaperId { get; set; }

        [DataType(DataType.MultilineText)]
        [MinLength(2, ErrorMessage = "The field Description must be a minimum length of '2' and maximum length of '500'.")]
        [MaxLength(500, ErrorMessage = "The field Description must be a minimum length of '2' and maximum length of '500'.")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Branch")]
        [Required]
        public int? BranchId { get; set; }

        [Display(Name = "Class")]
        public int? ClassId { get; set; }

        [Display(Name = "Batch")]
        public int? BatchId { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Batch")]
        public IEnumerable<SelectListItem> Batches { get; set; }

        [Display(Name = "Branch")]
        public string SelectedBranches { get; set; }

        [Display(Name = "Class")]
        public string SelectedClasses { get; set; }

        [Display(Name = "Batch")]
        public string SelectedBatches { get; set; }

        public string FileName { get; set; }

        [Display(Name = "File")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedOn { get; set; }

        public string CurrentUserRole { get; set; }

        public string BranchName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        [Display(Name = "Date")]
        public DateTime DailyPracticePaperDate { get; set; }

        [DataType(DataType.MultilineText)]
        [MinLength(2, ErrorMessage = "The Description must be a minimum length of '2' and maximum length of '500'.")]
        [MaxLength(500, ErrorMessage = "The Description must be a minimum length of '2' and maximum length of '500'.")]
        [Display(Name = "Attachment Description")]
        public string AttachmentDescription { get; set; }
    }
}