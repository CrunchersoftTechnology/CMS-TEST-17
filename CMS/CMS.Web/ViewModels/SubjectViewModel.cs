using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class SubjectViewModel
    {


        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int SubjectId { get; set; }

        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field Subject Name must be a minimum length of '2' and maximum length of '50'.")]
        [MinLength(2, ErrorMessage = "The field Subject Name must be a minimum length of '2' and maximum length of '50'.")]
        [Display(Name = "Subject Name")]
        public string Name { get; set; }
        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public string SelectedClasses { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }

    }
}