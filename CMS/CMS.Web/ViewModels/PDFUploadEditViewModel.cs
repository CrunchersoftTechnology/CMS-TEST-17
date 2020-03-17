using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class PDFUploadEditViewModel
    {
        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        public int PDFUploadId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field Title must be a minimum length of '3' and maximum length of '50'.")]
        [MinLength(3, ErrorMessage = "The field Title must be a minimum length of '3' and maximum length of '50'.")]
        public string Title { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Select PDF")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = "visible")]
        public bool IsVisible { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "PDF Category")]
        public string PDFCategoryName { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Required(ErrorMessage = "The PDFCategoryName field is required.")]
        public int PDFCategoryId { get; set; }

        [Display(Name = "PDF Category")]
        public IEnumerable<SelectListItem> PDFCategories { get; set; }

        [Display(Name = "App Notification (Student)")]
        public bool IsSend { get; set; }
    }
}