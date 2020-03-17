using CMS.Domain.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class QuestionGetViewModel
    {
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

        [Required(ErrorMessage = "Answer field is required.")]
        public string Answer { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Hint { get; set; }

        public bool IsQuestionAsImage { get; set; }
        
        public HttpPostedFileBase QuestionImageFile { get; set; }

        public bool IsOptionAsImage { get; set; }
        
        public HttpPostedFileBase OptionImageFile { get; set; }

        public bool IsHintAsImage { get; set; }
        
        public HttpPostedFileBase HintImageFile { get; set; }
        
        public string QuestionYear { get; set; }

        public string Numerical_Answer { get; set; }

        public string Unit { get; set; }

        [EnumDataType(typeof(QuestionLevel), ErrorMessage = "Level field is required.")]
        public QuestionLevel QuestionLevel { get; set; }

        [EnumDataType(typeof(QuestionType), ErrorMessage = "Type field is required.")]
        public QuestionType QuestionType { get; set; }

        public int QuestionId { get; set; }

        [Display(Name = "Question Image")]
        public string QuestionImagePath { get; set; }

        [Display(Name = "Option Image")]
        public string OptionImagePath { get; set; }

        [Display(Name = "Hint Image")]
        public string HintImagePath { get; set; }
    }
}