using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class TeacherViewModel
    {
        public int TId { get; set; }

        [RegularExpression("^[a-zA-Z]+[a-zA-Z ]*$", ErrorMessage = "First Name must be an alphabetic.")]
        [Display(Name = "First Name")]
        [Required]
        [MinLength(2, ErrorMessage = "The field First Name must be a minimum length of '2' and maximum length of '20'.")]
        [MaxLength(20, ErrorMessage = "The field First Name must be a minimum length of '2' and maximum length of '20'.")]
        public string FirstName { get; set; }

        [RegularExpression("^[a-zA-Z]+[a-zA-Z ]*$", ErrorMessage = "Last Name must be an alphabetic.")]
        [Display(Name = "Last Name")]
        [Required]
        [MinLength(2, ErrorMessage = "The field Last Name must be a minimum length of '2' and maximum length of '20'.")]
        [MaxLength(20, ErrorMessage = "The field Last Name must be a minimum length of '2' and maximum length of '20'.")]
        public string LastName { get; set; }

        [RegularExpression("^[a-zA-Z]+[a-zA-Z ]*$", ErrorMessage = "Middle Name must be an alphabetic.")]
        [Display(Name = "Middle Name")]
        [MinLength(1, ErrorMessage = "The field Middle Name must be a minimum length of '1' and maximum length of '20'.")]
        [MaxLength(20, ErrorMessage = "The field Middle Name must be a minimum length of '1' and maximum length of '20'.")]
        public string MiddleName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        [System.ComponentModel.DataAnnotations.Compare("Email")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Contact No.")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number must be numeric.")]
        [MinLength(10, ErrorMessage = "Contact must be 10 digit.")]
        [MaxLength(10, ErrorMessage = "Contact must be 10 digit.")]
        public string ContactNo { get; set; }

        public string UserId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "The Branch field is required.")]
        public int BranchId { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "The Branch field is required.")]
        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }


        public string CurrentUserRole { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "The field Qualification be a minimum length of '2' and maximum length of '20'.")]
        [MaxLength(50, ErrorMessage = "The field Qualification must be a minimum length of '2' and maximum length of '20'.")]
        public string Qualification { get; set; }
    }
}