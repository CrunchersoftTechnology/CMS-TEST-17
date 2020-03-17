using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class BoardMap : EntityTypeConfiguration<Board>
    {
        public BoardMap()
        {
            Property(x => x.Name).IsRequired().HasMaxLength(50);
        }
    }
}
