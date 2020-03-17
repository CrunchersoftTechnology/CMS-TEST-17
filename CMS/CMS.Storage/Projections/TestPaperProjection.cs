using CMS.Common.Enums;
using CMS.Domain.Models;
using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Projections
{
    public class TestPaperProjection
    {
        public int TestPaperId { get; set; }
        
        public string Title { get; set; }
        
        public int ClassId { get; set; }
        
        public bool TestTaken { get; set; }
        
        public TestType TestType { get; set; }
        
        public string DelimitedQuestionIds { get; set; }

        public string ClassName { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public string DelimitedChapterIds { get; set; }

        public DateTime CreatedOn { get; set; }

        public string SubjectName { get; set; }

        public int QuestionCount { get; set; }

        public int chapterId { get; set; }

        public int questionId { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartTime { get; set; }

        //public DateTime EndTime { get; set; }

        public int TimeDuration { get; set; }
    }
}
