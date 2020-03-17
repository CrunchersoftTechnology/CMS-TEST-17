using CMS.Domain.Models;
using System.Web;

namespace CMS.Domain.Storage.Projections
{
    public class QuestionProjection
    {
        public int QuestionId { get; set; }

        public string QuestionInfo { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string Option4 { get; set; }

        public string Answer { get; set; }

        public string Hint { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

        public int ChapterId { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionYear { get; set; }

        public string Numerical_Answer { get; set; }

        public string Unit { get; set; }

        public int? ClassId { get; set; }

        public int? SubjectId { get; set; }

        public bool IsQuestionAsImage { get; set; }

        public HttpPostedFileBase QuestionImageFile { get; set; }

        public bool IsOptionAsImage { get; set; }

        public HttpPostedFileBase OptionImageFile { get; set; }

        public bool IsHintAsImage { get; set; }
        
        public HttpPostedFileBase HintImageFile { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        public string ChapterName { get; set; }

        public string QuestionImagePath { get; set; }

        public string OptionImagePath { get; set; }

        public string HintImagePath { get; set; }
    }
}
