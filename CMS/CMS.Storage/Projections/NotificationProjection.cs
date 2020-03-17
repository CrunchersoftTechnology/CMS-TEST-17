using System;

namespace CMS.Domain.Storage.Projections
{
    public class NotificationProjection
    {
        public int NotificationId { get; set; }

        public string NotificationMessage { get; set; }

        public bool AllUser { get; set; }

        public string SelectedBranches { get; set; }

        public string SelectedClients { get; set; }

        public string SelectedClasses { get; set; }

        public string SelectedBatches { get; set; }

        public int StudentCount { get; set; }

        public int ParentCount { get; set; }

        public int TeacherCount { get; set; }

        public int BranchAdminCount { get; set; }

        public int ClientAdminCount { get; set; }

        public string Media { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? NotificationAutoDate { get; set; }

        public bool IsSend { get; set; }

        public string UserName { get; set; }
    }
}
