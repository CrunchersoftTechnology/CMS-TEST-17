using CMS.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Web.ViewModels
{
    public class PendingStudentAdmissionViewModel
    {
        public int AdmissionId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}