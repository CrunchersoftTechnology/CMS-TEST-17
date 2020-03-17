using CMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }

        [DataType(DataType.MultilineText)]
        [MinLength(2, ErrorMessage = "The field Message must be a minimum length of '2' and maximum length of '500'.")]
        [MaxLength(500, ErrorMessage = "The field Message must be a minimum length of '2' and maximum length of '500'.")]
        [Required]
        [Display(Name = "Notification Message")]
        public string NotificationMessage { get; set; }

        public bool AllUser { get; set; }

        public bool Student { get; set; }

        public bool Teacher { get; set; }

        public bool Parent { get; set; }

        public bool BranchAdmin { get; set; }

        public bool ClientAdmin { get; set; }


        public bool Email { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        [Display(Name = "Branch")]
        public int? BranchId { get; set; }

        [Display(Name = "Client")]
        public int? ClientId { get; set; }


        [Display(Name = "Class")]
        public int? ClassId { get; set; }

        [Display(Name = "Batch")]
        public int? BatchId { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Batch")]
        public IEnumerable<SelectListItem> Batches { get; set; }

        public string CurrentUserRole { get; set; }

        public string BranchName { get; set; }

        public string ClientName { get; set; }

        [Display(Name = "Class")]
        public string SelectedClasses { get; set; }

        [Display(Name = "Batch")]
        public string SelectedBatches { get; set; }

        [Display(Name = "Branch")]
        public string SelectedBranches { get; set; }

        [Display(Name = "Student Count")]
        public int StudentCount { get; set; }

        [Display(Name = "Parent Count")]
        public int ParentCount { get; set; }

        [Display(Name = "Teacher Count")]
        public int TeacherCount { get; set; }

        [Display(Name = "BranchAdmin Count")]
        public int BranchAdminCount { get; set; }

        [Display(Name = "ClientAdmin Count")]
        public int ClientAdminCount { get; set; }

        public string Media { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Notification Auto Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? NotificationAutoDate { get; set; }

    }
}