namespace CMS.Domain.Storage.Projections
{
    public class QuestionCountProjection
    {
        public string ChapterName { get; set; }
        public int Easy { get; set; }
        public int Medium { get; set; }
        public int Hard { get; set; }
        public int Numerical { get; set; }
        public int Theoretical { get; set; }
        public int NewPatternNumerical { get; set; }
        public int WithHint { get; set; }
        public int WithOutHint { get; set; }
        public int Asked { get; set; }
        public int NonAsked { get; set; }
        public int TotalQuestion { get; set; }
        public int ChapterId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
}
