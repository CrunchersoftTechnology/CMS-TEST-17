using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class PDFCategoryMap : EntityTypeConfiguration<PDFCategory>
    {
        public PDFCategoryMap()
        {
            Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}
