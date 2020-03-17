using CMS.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Domain.Storage.Projections
{
    public class AdmissionProjection
    {
        public int SId { get; set; }

        public int ClassId { get; set; }

        public int BoardId { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public DateTime DOJ { get; set; }

        //     public bool PickAndDrop { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime DOB { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public string Address { get; set; }

        public string Pin { get; set; }

        public string ParentContact { get; set; }

        public string StudentContact { get; set; }

        public virtual string SelectedSubject { get; set; }

        public bool IsWhatsApp { get; set; }

        public string MotherName { get; set; }

        public string SeatNumber { get; set; }

        public int SchoolId { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public string Email { get; set; }

        public bool status { get; set; }

        //   public string EmergencyContact { get; set; }

        public string RollNo { get; set; }

        public string ParentEmailId { get; set; }

        public int BatchId { get; set; }

        public string PaymentLists { get; set; }
    }
}
