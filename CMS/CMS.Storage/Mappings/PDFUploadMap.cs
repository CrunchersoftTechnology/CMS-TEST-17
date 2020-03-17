using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class PDFUploadMap : EntityTypeConfiguration<PDFUpload>
    {
        public PDFUploadMap()
        {
            //HasRequired(c => c.Class).
            //WithMany(t => t.PDFUploads).
            //Map(m => m.MapKey("ClassId")).
            //WillCascadeOnDelete(false);

            //Property(x => x.Title).IsRequired().HasMaxLength(100);
        }
    }
}
