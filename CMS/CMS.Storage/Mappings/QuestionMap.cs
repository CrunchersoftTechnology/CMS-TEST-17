using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class QuestionMap : EntityTypeConfiguration<Question>
    {
        public QuestionMap()
        {
            HasRequired(c => c.Chapter).
           WithMany(t => t.Questions).
           Map(m => m.MapKey("ChapterId")).
           WillCascadeOnDelete(false);
        }
    }
}
