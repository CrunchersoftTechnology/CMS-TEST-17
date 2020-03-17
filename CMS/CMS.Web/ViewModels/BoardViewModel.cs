using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace CMS.Web.ViewModels
{
    public class BoardViewModel
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int BoardId { get; set; }

        [RegularExpression("^[a-zA-Z&]+[a-zA-Z&\\- ]+$", ErrorMessage = "Board Name should contain A-Z, a-z,&, -.")]
        [Required]
        [MaxLength(50, ErrorMessage = "The field Board Name must be  a minimum length of '3' and maximum length of '50'.")]
        [MinLength(3, ErrorMessage = "The field Board Name must be  a minimum length of '3' and maximum length of '50'.")]
        [Display(Name = "Board Name")]
        public string Name { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }

    }
}