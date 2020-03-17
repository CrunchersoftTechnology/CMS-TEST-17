using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class PDFUploadDeleteViewModel
    {
        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        public int PDFUploadId { get; set; }

        [Required]
        public string Title { get; set; }

        public string FileName { get; set; }

        public HttpPostedFileBase FilePath { get; set; }

        public bool IsVisible { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Required(ErrorMessage = "The PDFCategoryName field is required.")]
        public int PDFCategoryId { get; set; }

        [Display(Name = "PDF Category")]
        public string PDFCategoryName { get; set; }

        [Display(Name = "PDF Category")]
        public IEnumerable<SelectListItem> PDFCategories { get; set; }

        [Display(Name = "App Notification (Student)")]
        public bool IsSend { get; set; }
    }
}