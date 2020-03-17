using CMS.Common;
using CMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class StudentViewModel
    {
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "The Batch field is required.")]
        [Display(Name = "Batch")]
        public int BatchId { get; set; }

        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Display(Name = "Board")]
        public int BoardId { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "The Gender field is required.")]
        public Gender Gender { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date of Joining")]
        [Required]
        public DateTime DOJ { get; set; }

        /*   [Display(Name = "Pick and Drop Facility")]
           public bool PickAndDrop { get; set; } */

        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "First Name must be an alphabetic.")]
        [Display(Name = "First Name")]
        [Required]
        [MinLength(2, ErrorMessage = "The field First Name must be a minimum length of '2' and maximum length of '20'.")]
        [MaxLength(30, ErrorMessage = "The field First Name must be a minimum length of '2' and maximum length of '20'.")]
        public string FirstName { get; set; }


        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Last Name must be an alphabetic.")]
        [Display(Name = "Last Name")]
        [Required]
        [MinLength(2, ErrorMessage = "The field Last Name must be a minimum length of '2' and maximum length of '20'.")]
        [MaxLength(30, ErrorMessage = "The field Last Name must be a minimum length of '2' and maximum length of '20'.")]
        public string LastName { get; set; }

        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Middle Name must be an alphabetic.")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Range(0, 9, ErrorMessage = "The Blood Group field is required.")]
        [Display(Name = "Blood Group")]
        public BloodGroup BloodGroup { get; set; }

        [Display(Name = "School Name")]
        public string SchoolName { get; set; }

        [DataType(DataType.MultilineText)]
        [MinLength(2, ErrorMessage = "The field Address must be a minimum length of '2' and maximum length of '200'.")]
        [MaxLength(200, ErrorMessage = "The field Address must be a minimum length of '2' and maximum length of '200'.")]
        [Required]
        public string Address { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Pin must be numeric.")]
        [MinLength(6, ErrorMessage = "The field Pin must be a '6' digit.")]
        [MaxLength(6, ErrorMessage = "The field Pin must be a '6' digit.")]
        public string Pin { get; set; }

        [Display(Name = "Parent Contact")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Parent Contact must be numeric.")]
        [MaxLength(10, ErrorMessage = "Contact must be 10 digit.")]
        [MinLength(10, ErrorMessage = "Contact must be 10 digit.")]
        public string ParentContact { get; set; }

        [Display(Name = "Student Contact")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Student Contact must be numeric.")]
        [MaxLength(10, ErrorMessage = "Contact must be 10 digit.")]
        [MinLength(10, ErrorMessage = "Contact must be 10 digit.")]
        [Required]
        public string StudentContact { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        [System.ComponentModel.DataAnnotations.Compare("Email")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Subject")]
        public IEnumerable<SelectListItem> Subjects { get; set; }

        [Display(Name = "Board")]
        public IEnumerable<SelectListItem> Boards { get; set; }

        [Display(Name = "Batches")]
        public IEnumerable<SelectListItem> Batches { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total Fee can't be zero.")]
        [Display(Name = "Total Fee")]
        [Required]
        public decimal TotalFees { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Discount Can't be Negative")]
        [Display(Name = "Discount")]
        public decimal Discount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Final Fee can't be zero.exceeded amount of discount")]
        [Display(Name = "Fee (After Discount)")]
        [Required]
        public decimal FinalFees { get; set; }

        [Display(Name = "Select Photo")]
        public string PhotoPath { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Subject")]
        public string SubjectName { get; set; }

        [Display(Name = "Board")]
        public string BoardName { get; set; }

        [Display(Name = "Batch")]
        public string BatchName { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "The Subject field is required.")]
        [Display(Name = "Selected Subject")]
        public string SelectedSubject { get; set; }

        public HttpPostedFileBase PhotoFilePath { get; set; }

        [Display(Name = "WhatsApp Available")]
        public bool IsWhatsApp { get; set; }

        public int SId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Punch Id must be an numeric.")]
        [Required(ErrorMessage = "The Punch Id field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Punch Id can't be zero.")]
        [Display(Name = "Punch ID")]
        public int PunchId { get; set; }

        [Display(Name = "Mother Name")]
        [MaxLength(100)]
        public string MotherName { get; set; }

        /*   [Display(Name = "VAN Area")]
           [MaxLength(200)]
           public string VANArea { get; set; } */

        [Display(Name = "Seat Number")]
        [MaxLength(200)]
        public string SeatNumber { get; set; }

        /*   [Range(0, (double)decimal.MaxValue, ErrorMessage = "VAN Fee Can't be Negative")]
           [Display(Name = "VAN Fee")]
           [Required]
           public decimal VANFee { get; set; }*/

        [Required]
        public int SchoolId { get; set; }

        [Display(Name = "School")]
        public IEnumerable<SelectListItem> Schools { get; set; }

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

        public string ImageData { get; set; }

        public int? IsIdExits { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Paid Fee Can't be Negative")]
        [Display(Name = "Paid Fee")]
        public decimal PaidFee { get; set; }

        [RegularExpression("^[a-zA-Z0-9\\- ]+$", ErrorMessage = "Receipt Number should contain a-z, A-Z, 0-9 and dash.")]
        [MinLength(1, ErrorMessage = "The field Receipt Number must be a minimum length of '1' and maximum length of '50'.")]
        [MaxLength(50, ErrorMessage = "The field Receipt Number must be a minimum length of '1' and maximum length of '50'.")]
        [Display(Name = "Receipt No.")]
        public string ReceiptNumber { get; set; }

        [RegularExpression("^[a-zA-Z0-9\\- ]+$", ErrorMessage = "Receipt Book Number should contain a-z, A-Z, 0-9 and dash.")]
        [MinLength(1, ErrorMessage = "The field Receipt Book Number must be a minimum length of '1' and maximum length of '50'.")]
        [MaxLength(50, ErrorMessage = "The field Receipt Book Number must be a minimum length of '1' and maximum length of '50'.")]
        [Display(Name = "Receipt Book No")]
        public string ReceiptBookNumber { get; set; }

        [Display(Name = "Roll No")]
        public string RollNo { get; set; }

        [Display(Name = "Parent Email")]
        [EmailAddress]
        public string ParentEmailId { get; set; }

        [Display(Name = "Payment No")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Payment No must be numeric.")]
        //[Range(0, 12, ErrorMessage = "Enter number between 0 to 12")]
        public PaymentNo PaymentNo { get; set; }

        public string PaymentLists { get; set; }

        public string PaymentErrorMessage { get; set; }
    }
}