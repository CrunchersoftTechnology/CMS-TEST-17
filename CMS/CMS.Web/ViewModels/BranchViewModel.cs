using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class BranchViewModel
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int BranchId { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+[a-zA-Z0-9\\-., ]*$", ErrorMessage = "Name should contain A-Z, a-z,0-9, dash, comma.")]
        [MinLength(5, ErrorMessage = "The field Name must be a minimum length of '5' and maximum length of '100'.")]
        [MaxLength(100, ErrorMessage = "The field Name must be a minimum length of '5' and maximum length of '100'.")]
        public string Name { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "The field Address must be a minimum length of '5' and maximum length of '150'.")]
        [MaxLength(150, ErrorMessage = "The field Address must be a minimum length of '5' and maximum length of '150'.")]
        public string Address { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }


    }
}