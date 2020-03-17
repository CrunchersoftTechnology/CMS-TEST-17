using CMS.Common;
using CMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class StudentFeedbackViewModel
    {
        public int StudentFeedbackId { get; set; }

        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        [System.ComponentModel.DataAnnotations.Compare("Email")]
        public string ConfirmEmail { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "The field Contact must be length of '10'")]
        [MinLength(10, ErrorMessage = "The field Contact must be length of '10'")]
        public string Contact { get; set; }

        [Display(Name = "Teacher")]
        public string UserId { get; set; }

        [Display(Name = "Teacher")]
        public IEnumerable<SelectListItem> Teacher { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Message")]
        [MinLength(2, ErrorMessage = "The field Title must be a minimum length of '2' and maximum length of '500'.")]
        [MaxLength(500, ErrorMessage = "The field Title must be a minimum length of '2' and maximum length of '500'.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "The Feedback For field is required.")]
        [Display(Name = "Feedback For")]
        public FeedbackFor FeedbackFor { get; set; }

        public string Status { get; set; }

        public string TeacherName { get; set; }

        [Display(Name = "Rating")]
        public int Rating { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}