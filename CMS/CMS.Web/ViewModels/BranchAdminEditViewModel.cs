using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class BranchAdminEditViewModel
    {
        public int BId { get; set; }

        [Required(ErrorMessage = "The Branch field is required.")]
        public int BranchId { get; set; }

        public string UserId { get; set; }

        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Branch Admin Name must be an alphabetic.")]
        [Display(Name = "Branch Admin Name")]
        [Required]
        [MinLength(2, ErrorMessage = "The field Branch Admin Name must be a minimum length of '2' and maximum length of '100'.")]
        [MaxLength(100, ErrorMessage = "The field Branch Admin Name must be a minimum length of '2' and maximum length of '100'.")]
        public string Name { get; set; }

        [Display(Name = "Active Status")]
        public bool Active { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "Contact Number")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number must be numeric.")]
        [MinLength(10, ErrorMessage = "Contact Number must be 10 digit.")]
        [MaxLength(10, ErrorMessage = "Contact Number must be 10 digit.")]
        public string ContactNo { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }
    }
}