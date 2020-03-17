namespace CMS.Domain.Storage.Projections
{
    public class ChapterProjection
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }
        public int ChapterId { get; set; }
        public int SubjectId { get; set; }
        public string ChapterName { get; set; }
        public string SubjectName { get; set; }
        public int Weightage { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
    }
}
