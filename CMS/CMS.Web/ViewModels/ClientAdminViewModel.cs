using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class ClientAdminViewModel
    {
        public int CId { get; set; }

        [Required(ErrorMessage = "The Client field is required.")]
        public int ClientId { get; set; }

        public string UserId { get; set; }

        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Client Admin Name must be an alphabetic.")]
        [Display(Name = "Client Admin Name")]
        [Required]
        [MinLength(2, ErrorMessage = "The field Client Admin Name must be a minimum length of '2' and maximum length of '100'.")]
        [MaxLength(100, ErrorMessage = "The field Client Admin Name must be a minimum length of '2' and maximum length of '100'.")]
        public string Name { get; set; }

        [Display(Name = "Active Status")]
        public bool Active { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        [System.ComponentModel.DataAnnotations.Compare("Email")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Contact Number")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number must be numeric.")]
        [MaxLength(10, ErrorMessage = "Contact Number must be 10 digit.")]
        [MinLength(10, ErrorMessage = "Contact Number must be 10 digit.")]
        public string ContactNo { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }
    }
}