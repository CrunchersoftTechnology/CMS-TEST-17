namespace CMS.Domain.Storage.Projections
{
    public class MasterFeeProjection
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int MasterFeeId { get; set; }

        public string Year { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public decimal Fee { get; set; }
    }
}
