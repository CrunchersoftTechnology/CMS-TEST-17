using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CMS.Web.ViewModels
{
    public class QuestionImageViewModel
    {
        public int QuestionId { get; set; }

        [Required]
        public HttpPostedFileBase QuestionImage { get; set; }
    }
}