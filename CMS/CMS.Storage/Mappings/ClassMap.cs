using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class ClassMap : EntityTypeConfiguration<Class>
    {
        public ClassMap()
        {
            Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}
