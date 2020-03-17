using CMS.Domain.Models;
using System.Data.Entity.ModelConfiguration;

namespace CMS.Domain.Storage.Mappings
{
    public class StudentMap : EntityTypeConfiguration<Student>
    {
        public StudentMap()
        {
        }
    }
}
