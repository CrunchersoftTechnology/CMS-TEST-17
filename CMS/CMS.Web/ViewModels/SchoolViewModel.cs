using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class SchoolViewModel
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int SchoolId { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9&]+[a-zA-Z0-9&\\-.', ]*$", ErrorMessage = "Name should contain A-Z, a-z,0-9, &, dash, comma,Apostrophe.")]
        [MinLength(3, ErrorMessage = "The field Name must be a minimum length of '3' and maximum length of '200'.")]
        [MaxLength(200, ErrorMessage = "The field Name must be a minimum length of '3' and maximum length of '200'.")]
        public string Name { get; set; }

        [Display(Name = "Center Number")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Center Number should contain A-Z, a-z, 0-9.")]
        [MinLength(1, ErrorMessage = "The field Center Number must be a minimum length of '1' and maximum length of '50'.")]
        [MaxLength(50, ErrorMessage = "The field Center Number must be a minimum length of '1' and maximum length of '50'.")]
        public string CenterNumber { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }

    }
}