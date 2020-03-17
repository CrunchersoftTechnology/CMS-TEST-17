using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class BatchMap : EntityTypeConfiguration<Batch>
    {
        public BatchMap()
        {
            //HasRequired(c => c.Subject).
            //WithMany(t => t.Batches).
            //Map(m => m.MapKey("SubjectId")).
            //WillCascadeOnDelete(false);

            //Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}
