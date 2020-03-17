using CMS.Domain.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class QuestionDataViewModel
    {
        //[Required]
        [DataType(DataType.MultilineText)]
        public string QuestionInfo { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionA { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionB { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionC { get; set; }

        [DataType(DataType.MultilineText)]
        public string OptionD { get; set; }

        //[EnumDataType(typeof(Answer), ErrorMessage = "Answer field is required.")]
        [Required(ErrorMessage = "Answer field is required.")]
        public string Answer { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        public string Hint { get; set; }

        [Display(Name = "Question Image")]
        public bool IsQuestionAsImage { get; set; }

       // [CustomAttributes.FileExtensions("csv,xls", ErrorMessage = "Select only csv,xls.")]
        public HttpPostedFileBase QuestionImageFile { get; set; }

        [Display(Name = "Option Image")]
        public bool IsOptionAsImage { get; set; }

        //[CustomAttributes.FileExtensions("csv,xls", ErrorMessage = "Select only csv,xls.")]
        //[FileExtensions(Extensions ="jpg, png, gif", ErrorMessage ="Please upload valid format")]
        public HttpPostedFileBase OptionImageFile { get; set; }

        [Display(Name = "Hint Image")]
        public bool IsHintAsImage { get; set; }

        //[CustomAttributes.FileExtensions("csv,xls", ErrorMessage = "Select only csv,xls.")]
        public HttpPostedFileBase HintImageFile { get; set; }

        [Required(ErrorMessage = "Year field is required.")]
        public string QuestionYear { get; set; }

        [Required(ErrorMessage = "Numerical_Answer field is required.")]
        public string Numerical_Answer { get; set; }

        [Required(ErrorMessage = "Unit field is required.")]
        public string Unit { get; set; }

        [EnumDataType(typeof(QuestionLevel), ErrorMessage = "Level field is required.")]
        public QuestionLevel QuestionLevel { get; set; }

        [EnumDataType(typeof(QuestionType), ErrorMessage = "Type field is required.")]
        public QuestionType QuestionType { get; set; }

        [Display(Name = "Classes")]
        public IEnumerable<SelectListItem> Classes { get; set; }

        [Display(Name = "Classes")]
        [Required(ErrorMessage = "Class field is required.")]
        public int? ClassId { get; set; }

        [Display(Name = "Subject")]
        public IEnumerable<SelectListItem> Subjects { get; set; }

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Subject field is required.")]
        public int? SubjectId { get; set; }

        [Display(Name = "Chapters")]
        [Required(ErrorMessage = "Chapter field is required.")]
        public int ChapterId { get; set; }

        [Display(Name = "Chapters")]
        public IEnumerable<SelectListItem> Chapters { get; set; }

        public int QuestionId { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        public string ChapterName { get; set; }

        [Display(Name = "Question Image")]
        public string QuestionImagePath { get; set; }

        [Display(Name = "Option Image")]
        public string OptionImagePath { get; set; }

        [Display(Name = "Hint Image")]
        public string HintImagePath { get; set; }
    }
}