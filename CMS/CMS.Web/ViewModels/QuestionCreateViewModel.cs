using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CMS.Web.ViewModels
{
    public class QuestionCreateViewModel
    {
        public int QuestionId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string QuestionInfo { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionA { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionB { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionC { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionD { get; set; }

        [Required]
        public string Answer { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Hint { get; set; }

        public HttpPostedFileBase QuestionImage { get; set; }

        public HttpPostedFileBase OptionImage { get; set; }

        public HttpPostedFileBase HintImage { get; set; }

        [MaxLength(100, ErrorMessage = "The field Year must be a maximum length of '100'.")]
        public string Year { get; set; }

        [MaxLength(100, ErrorMessage = "The field Numerical_Answer must be a maximum length of '100'.")]
        public string Numerical_Answer { get; set; }

        [MaxLength(100, ErrorMessage = "The field Unit must be a maximum length of '100'.")]
        public string Unit { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int ChapterId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int ClassId { get; set; }
    }
}