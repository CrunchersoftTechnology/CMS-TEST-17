using CMS.Common.Enums;
using CMS.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Student : AuditableEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SId { get; set; }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public virtual IEnumerable<Installment> Installments { get; set; }

        public int BatchId { get; set; }

        public int ClassId { get; set; }

        public int BoardId { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public DateTime DOJ { get; set; }

       // public bool PickAndDrop { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime DOB { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public string Address { get; set; }

        public string Pin { get; set; }

        public string ParentContact { get; set; }

        public string StudentContact { get; set; }

        public decimal TotalFees { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalFees { get; set; }

     //   public string PhotoPath { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("BatchId")]
        public virtual Batch Batches { get; set; }

        [ForeignKey("BoardId")]
        public virtual Board Board { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }

        public Student()
        {
            Subjects = new HashSet<Subject>();
        }

        public virtual string SelectedSubject { get; set; }

        public bool IsWhatsApp { get; set; }

        public int PunchId { get; set; }

        public string MotherName { get; set; }

     /*   public decimal VANFee { get; set; }

        [MaxLength(100)]
        public string VANArea { get; set; }*/

        [MaxLength(50)]
        public string SeatNumber { get; set; }

        public int SchoolId { get; set; }

        [ForeignKey("SchoolId")]
        public virtual School School { get; set; }

        public virtual ICollection<School> Schools { get; set; }

        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        
        public string studentAppPlayerId { get; set; }

        public string parentAppPlayerId { get; set; }

        public string RollNo { get; set; }

        public string ParentEmailId { get; set; }

        public string PaymentLists { get; set; }
    }
}
