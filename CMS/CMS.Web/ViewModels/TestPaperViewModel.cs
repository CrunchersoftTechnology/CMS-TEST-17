using CMS.Common;
using CMS.Common.Enums;
using CMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class TestPaperViewModel
    {
        public int TestPaperId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Display(Name = "Test Taken")]
        public bool TestTaken { get; set; }

        [Required]
        [Display(Name = "Test Type")]
        public TestType TestType { get; set; }

        [Required]
        public string DelimitedQuestionIds { get; set; }

        [Display(Name = "Class")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        public string DelimitedChapterIds { get; set; }

        [Display(Name ="Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedOn { get; set; }

        public string SubjectName { get; set; }

        public int QuestionCount { get; set; }

    }
}