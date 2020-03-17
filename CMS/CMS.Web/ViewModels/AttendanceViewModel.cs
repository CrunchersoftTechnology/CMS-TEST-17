using CMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class AttendanceViewModel
    {
        public int AttendanceId { get; set; }

        [Required(ErrorMessage = "The Class field is required")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "The Batch field is required")]
        public int BatchId { get; set; }

        [DataType(DataType.Date)]
     //   [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(200, ErrorMessage = "The field Activity must be  a minimum length of '5' and maximum length of '200'.")]
        [MinLength(5, ErrorMessage = "The field Activity must be  a minimum length of '5' and maximum length of '200'.")]
        [Required(ErrorMessage = "The Activity field is required")]
        public string Activity { get; set; }

        [Required(ErrorMessage = "The Teacher Name field is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Atleast one student must be present.")]
        [Display(Name = "Student List")]
        public string StudentAttendence { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Batch")]
        public IEnumerable<SelectListItem> Batches { get; set; }

        [Display(Name = "Teacher Name")]
        public IEnumerable<SelectListItem> Teachers { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Batch")]
        public string BatchName { get; set; }

        [Display(Name = "Teacher Name")]
        public string TeacherName { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "The Branch field is required.")]
        public int BranchId { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "The Client field is required.")]
        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }

        public string CurrentUserRole { get; set; }

        [Display(Name = "Subject")]
        public string SubjectName { get; set; }

        public bool SMS { get; set; }

        [Display(Name = "Email")]
        public bool Email { get; set; }

        [Display(Name = "App Notification")]
        public bool AppNotification { get; set; }

        [Display(Name = "Status")]
        public string SelectedAttendance { get; set; }

        [Display(Name = "In Time")]
        public string InTime { get; set; }

        [Display(Name = "Out Time")]
        public string OutTime { get; set; }
    }
}