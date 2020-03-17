using CMS.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common.GridModels
{
    public class StudentGridModel
    {
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Board")]
        public string BoardName { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "School")]
        public string SchoolName { get; set; }

        [Display(Name = "Parent Contact")]
        public string ParentContact { get; set; }

        [Display(Name = "Student Contact")]
        public string StudentContact { get; set; }

        [Display(Name = "Roll No")]
        public string RollNo { get; set; }

        public string Email { get; set; }

        [Display(Name = "Total Fees")]
        public decimal TotalFees { get; set; }

        /*   [Exclude]
           public string PhotoPath { get; set; } */

        [Display(Name = "Branch")]
        public string BranchName { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }

        public DateTime DOJ { get; set; }

        public Gender Gender { get; set; }

        public bool IsActive { get; set; }

        public int ClassId { get; set; }

        public string BatchName { get; set; }

        public string Address { get; set; }

        public string pin { get; set; }

        public DateTime DOB { get; set; }

        //    public bool PickAndDrop { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public string SeatNumber { get; set; }

        /*   public decimal VANFee { get; set; }

           public string VANArea { get; set; } */

        public decimal Discount { get; set; }

        public decimal FinalFees { get; set; }

        public int PunchId { get; set; }

        public bool IsWhatsApp { get; set; }

        public int BranchId { get; set; }

        public int ClientId { get; set; }

        [Exclude]
        public string Action { get; set; }

        public DateTime Createdon { get; set; }

    }
}
