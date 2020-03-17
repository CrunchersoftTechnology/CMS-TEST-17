using CMS.Domain.Models;

namespace CMS.Domain.Storage.Projections
{
    public class QuestionDetailWithCountProjection : Question
    {
        public int MediumCount { get; set; }
        public int EasyCount { get; set; }
        public int HardCount { get; set; }
        public int TheoreticalCount { get; set; }
        public int NumericalCount { get; set; }
        public int NewPatternNumericalCount { get; set; }
        public int AskedCount { get; set; }
        public int NotAskedCount { get; set; }
        public int HintCount { get; set; }
        public int WithoutHintCount { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TotalCount { get; set; }
        public int UsedCount { get; set; }
    }
}
