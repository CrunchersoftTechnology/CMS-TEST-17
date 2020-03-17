using CMS.Common.Enums;
using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class TestPaper : AuditableEntity
    {
        public int TestPaperId { get; set; }

        public string Title { get; set; }

        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public bool TestTaken { get; set; }

        public TestType TestType { get; set; }

        public string DelimitedQuestionIds { get; set; }

        public string DelimitedChapterIds { get; set; }

        public string SubjectName { get; set; }

        public int QuestionCount { get; set; }
    }
}
