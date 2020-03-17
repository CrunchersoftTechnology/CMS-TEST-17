using System;

namespace CMS.Domain.Storage.Projections
{
    public class BatchProjection
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
    }
}
