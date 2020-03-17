using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class PDFCategoryViewModel
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int PDFCategoryId { get; set; }

        [RegularExpression("^[a-zA-Z0-9]+[a-zA-Z0-9\\- ]+$", ErrorMessage = "PDF Category should contain A-Z, a-z,0-9, -.")]
        [Required]
        [MaxLength(50, ErrorMessage = "The field PDF Category Name must be a minimum length of '2' and maximum length of '50'.")]
        [MinLength(2, ErrorMessage = "The field PDF Category Name must be a minimum length of '2' and maximum length of '50'.")]
        [Display(Name = "PDF Category Name")]
        public string Name { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }
    }
}