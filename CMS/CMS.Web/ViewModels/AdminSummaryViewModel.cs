using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Web.ViewModels
{
    public class AdminSummaryViewModel
    {
        public int BranchesCount { get; set; }
        public int ClientsCount { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int BatchesCount { get; set; }
        public string PendingAdmissionCount { get; set; }

        public string CurrentUserRole { get; set; }
    }
}