namespace CMS.Domain.Storage.Projections
{
    public class MachineProjection
    {
        public int MachineId { get; set; }

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }
    }
}
