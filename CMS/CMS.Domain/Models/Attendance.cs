using CMS.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Attendance : AuditableEntity
    {
        public int AttendanceId { get; set; }

        public int ClassId { get; set; }

        public int BatchId { get; set; }

        public DateTime Date { get; set; }

        public string Activity { get; set; }

        public string UserId { get; set; }

        public string StudentAttendence { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batch { get; set; }

        [ForeignKey("UserId")]
        public virtual Teacher Teacher { get; set; }

        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public bool IsManual { get; set; }

        public bool IsSend { get; set; }

        public string InTime { get; set; }

        public string OutTime { get; set; }
    }
}
