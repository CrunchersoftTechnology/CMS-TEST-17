using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    class ChapterMap : EntityTypeConfiguration<Chapter>
    {
        public ChapterMap()
        {
            HasRequired(c => c.Subject).
            WithMany(t => t.Chapters).
            Map(m => m.MapKey("SubjectId")).
            WillCascadeOnDelete(false);
            Property(c => c.Name).IsRequired().HasMaxLength(50);
            Property(c => c.Weightage).IsRequired();
        }
    }
}
