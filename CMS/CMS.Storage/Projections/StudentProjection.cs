using CMS.Common.Enums;
using CMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace CMS.Domain.Storage.Projections
{
    public class StudentProjection
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int SubjectId { get; set; }

        public int BatchId { get; set; }

        public int ClassId { get; set; }

        public int BoardId { get; set; }

        public Gender Gender { get; set; }

        public DateTime DOJ { get; set; }

        // public bool PickAndDrop { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime DOB { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public string SchoolName { get; set; }

        public string Address { get; set; }

        public string Pin { get; set; }

        public string ParentContact { get; set; }

        public string StudentContact { get; set; }

        public string RollNo { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public decimal TotalFees { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalFees { get; set; }

        public string PhotoPath { get; set; }

        public bool IsActive { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        public string BoardName { get; set; }

        public string UserId { get; set; }

        public bool IsWhatsApp { get; set; }

        public string SelectedSubjects { get; set; }

        public HttpPostedFileBase PhotoFilePath { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }

        public string SelectedSubject { get; set; }

        public int SId { get; set; }

        public int PunchId { get; set; }

        public string MotherName { get; set; }

        //   public string VANArea { get; set; }

        public string SeatNumber { get; set; }

        //   public decimal VANFee { get; set; }

        public int SchoolId { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public string studentAppPlayerId { get; set; }

        public string parentAppPlayerId { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> StudentSubjects { get; set; }

        //   public string EmergencyContact { get; set; }

        public string ParentEmailId { get; set; }

        public string BatchName { get; set; }

        public string PaymentLists { get; set; }

        public DateTime Date { get; set; }

        // public string PaymentList { get; set; }


    }
}
