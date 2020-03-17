using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Question : AuditableEntity
    {
        public int QuestionId { get; set; }

        public string QuestionInfo { get; set; }

        public string OptionA { get; set; }

        public string OptionB { get; set; }

        public string OptionC { get; set; }

        public string OptionD { get; set; }

        public string Answer { get; set; }

        public string Hint { get; set; }

        public QuestionLevel QuestionLevel { get; set; }

       // public int SubjectId { get; set; }

      // [ForeignKey("SubjectId")]
       //public virtual Subject Subject { get; set; }

        public int ChapterId { get; set; }

        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; }

      // public int Topic_Id { get; set; }

        public bool IsQuestionAsImage { get; set; }

        public string QuestionImagePath { get; set; }

        public bool IsOptionAsImage { get; set; }

        public string OptionImagePath { get; set; }

        public bool IsHintAsImage { get; set; }

        public string HintImagePath { get; set; }

        public int AttepmtCount { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionYear { get; set; }

        public string Numerical_Answer  { get; set; }

        public string Unit { get; set; }
    }

    public enum QuestionLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public enum QuestionType
    {
        Theoretical = 1,
        Numerical = 2,
        NewPatternNumerical=3

    }

    public enum Answer
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4
    }
}
