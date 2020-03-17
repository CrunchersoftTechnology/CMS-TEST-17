namespace CMS.Domain.Storage.Projections
{
    public class SubjectProjection
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int SubjectId { get; set; }

        public string Name { get; set; }

        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public string SelectedClasses { get; set; }
    }
}
