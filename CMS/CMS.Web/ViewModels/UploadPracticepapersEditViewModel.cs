using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class UploadPracticepapersEditViewModel
    {
        public int UploadPracticepapersId { get; set; }

        [Required(ErrorMessage = "The Board field is required.")]
        public int BoardId { get; set; }

        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "The Subject field is required.")]
        public int SubjectId { get; set; }

        public string BoardName { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [MinLength(2, ErrorMessage = "The field Title must be a minimum length of '2' and maximum length of '250'.")]
        [MaxLength(250, ErrorMessage = "The field Title must be a minimum length of '2' and maximum length of '250'.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Practicepapers Date field is required.")]
        [Display(Name = "Practicepapers Date")]
        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Logo Name")]
        public string LogoName { get; set; }

        [Display(Name = "File")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = "Logo")]
        public HttpPostedFileBase LogoPath { get; set; }

        [Display(Name = "Board")]
        public IEnumerable<SelectListItem> Boards { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Subject")]
        public IEnumerable<SelectListItem> Subjects { get; set; }

        [Display(Name = "Visible")]
        public bool IsVisible { get; set; }
    }
}