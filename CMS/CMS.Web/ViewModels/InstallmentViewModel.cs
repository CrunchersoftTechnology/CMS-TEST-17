using CMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class InstallmentViewModel
    {
        public int InstallmentId { get; set; }

        [Required(ErrorMessage = "The Student Name field is required.")]
        public string UserId { get; set; }

        [Display(Name = "Student")]
        public IEnumerable<SelectListItem> Students { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Display(Name = "Total Fee")]
        public string TotalFee { get; set; }

        [Display(Name = "Remaining Fee")]
        public decimal RemainingFee { get; set; }

        [Required]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "Amount Can't be Zero or Negative")]
        [Display(Name = "Payment")]
        public decimal Payment { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = " Received/Total fee")]
        public decimal FinalFee { get; set; }

        public string Email { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9\\- ]+$", ErrorMessage = "Receipt Number should contain a-z, A-Z, 0-9 and dash.")]
        [MinLength(1, ErrorMessage = "The field Receipt Number must be a minimum length of '1' and maximum length of '50'.")]
        [MaxLength(50, ErrorMessage = "The field Receipt Number must be a minimum length of '1' and maximum length of '50'.")]
        [Display(Name = "Receipt No.")]
        public string ReceiptNumber { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9\\- ]+$", ErrorMessage = "Receipt Book Number should contain a-z, A-Z, 0-9 and dash.")]
        [MinLength(1, ErrorMessage = "The field Receipt Book Number must be a minimum length of '1' and maximum length of '50'.")]
        [MaxLength(50, ErrorMessage = "The field Receipt Book Number must be a minimum length of '1' and maximum length of '50'.")]
        [Display(Name = "Receipt Book No")]
        public string ReceiptBookNumber { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }  

        [Required(ErrorMessage = "The Branch field is required.")]
        public int BranchId { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "The Client field is required.")]
        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }


        public string CurrentUserRole { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "Email")]
        public bool EmailSend { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        public string StudentContact { get; set; }

        public string ParentContact { get; set; }

        public string ParentAppPlayerId { get; set; }

        public decimal RemainingFeeFinal { get; set; }

        [Display(Name = "Payment No.")]
        public int InstallmentNo { get; set; }

        public string StudBatch { get; set;}

        public IEnumerable<string> StudentBatches { get; set; }
    }
}