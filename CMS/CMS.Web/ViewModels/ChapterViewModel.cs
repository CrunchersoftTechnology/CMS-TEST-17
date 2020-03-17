using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class ChapterViewModel
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int ChapterId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field Chapter Name must be a minimum length of '3' and maximum length of '50'.")]
        [MinLength(3, ErrorMessage = "The field Chapter Name must be a minimum length of '3' and maximum length of '50'.")]
        [Display(Name = "Chapter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Subject field is required.")]
        public int SubjectId { get; set; }

        [Required]
        [Range(1, 100)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Weightage must be numeric.")]
        [Display(Name = "Weightage(%)")]
        public int Weightage { get; set; }

        [Display(Name = "Subject")]
        public IEnumerable<SelectListItem> Subjects { get; set; }

        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }


        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }
    }
}