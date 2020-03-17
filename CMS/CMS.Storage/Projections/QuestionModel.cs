using CMS.Domain.Models;

namespace CMS.Domain.Storage.Projections
{
    public class QuestionModel
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

       // public int SubjectId { get; set; }

        public int ChapterId { get; set; }

      //  public int Topic_Id { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionYear { get; set; }

        public string Numerical_Answer { get; set; }

        public string Unit { get; set; }

        public bool IsQuestionAsImage { get; set; }

        public bool IsOptionAsImage { get; set; }

        public bool IsHintAsImage { get; set; }

        public string QuestionImagePath { get; set; }

        public string OptionImagePath { get; set; }

        public string HintImagePath { get; set; }
    }
}
