using System;

namespace CMS.Common.GridModels
{
    public class MachineGridModel
    {
        public int MachineId { get; set; }

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
