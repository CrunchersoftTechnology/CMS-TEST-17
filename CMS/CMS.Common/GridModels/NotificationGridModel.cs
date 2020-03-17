using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common.GridModels
{
    public class NotificationGridModel
    {
        public int NotificationId { get; set; }

        public string NotificationMessage { get; set; }

        public bool AllUser { get; set; }

        public int StudentCount { get; set; }

        public int ParentCount { get; set; }

        public int TeacherCount { get; set; }

        public int BranchAdminCount { get; set; }

        public int ClientAdminCount { get; set; }

        public string Media { get; set; }

        public DateTime CreatedOn { get; set; }

        [Exclude]
        public string Action { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }

    }
}
