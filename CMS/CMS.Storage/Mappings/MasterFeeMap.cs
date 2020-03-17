using CMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Mappings
{
    public class MasterFeeMap : EntityTypeConfiguration<MasterFee>
    {
        public MasterFeeMap()
        {
            HasRequired(c => c.Subject).
            WithMany(t => t.MasterFees).
            Map(m => m.MapKey("SubjectId")).
            WillCascadeOnDelete(false);

            Property(m => m.Fee).IsRequired();
            Property(m => m.Year).IsRequired();
        }
    }
}
