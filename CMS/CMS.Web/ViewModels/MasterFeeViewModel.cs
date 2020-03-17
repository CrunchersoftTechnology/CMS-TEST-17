using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class MasterFeeViewModel
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int MasterFeeId { get; set; }

        [Required]
        public string Year { get; set; }

        [Required(ErrorMessage = "The Class field is required.")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "The Subject field is required.")]
        public int SubjectId { get; set; }

        [Required]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "Amount Can't be Zero or Negative.")]
        public decimal Fee { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Subject")]
        public IEnumerable<SelectListItem> Subjects { get; set; }

        [Display(Name = "Subject")]
        public string SubjectName { get; set; }

        public string CurrentUserRole { get; set; }


        [Display(Name = "Client")]
        public IEnumerable<SelectListItem> Clients { get; set; }

    }
}