using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CMS.Web.ViewModels
{
    public class BrochureViewModel
    {
        [Required(ErrorMessage = " The Brochure field is required.")]
        public HttpPostedFileBase FilePath { get; set; }
    }
}