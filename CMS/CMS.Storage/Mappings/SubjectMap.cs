using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class SubjectMap : EntityTypeConfiguration<Subject>
    {
        public SubjectMap()
        {
            HasRequired(c => c.Class).
            WithMany(t => t.Subjects).
            Map(m => m.MapKey("ClassId")).
            WillCascadeOnDelete(false);

            Property(x => x.Name).IsRequired().HasMaxLength(50);         
        }
    }
}
