using CMS.Common.Enums;
using System;

namespace CMS.Common.GridModels
{
    public class TestPaperGridModel
    {
        public int TestPaperId { get; set; }

        public string Title { get; set; }

        public int ClassId { get; set; }

        public bool TestTaken { get; set; }

        public TestType TestType { get; set; }

        public string DelimitedQuestionIds { get; set; }

        public string ClassName { get; set; }

        public string DelimitedChapterIds { get; set; }

        public DateTime CreatedOn { get; set; }
        [Exclude]
        public string Action { get; set; }

        public string SubjectName { get; set; }

        public int QuestionCount { get; set; }
    }
}
