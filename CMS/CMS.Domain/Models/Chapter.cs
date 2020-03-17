using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Chapter : AuditableEntity
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int ChapterId { get; set; }
        
        public string Name { get; set; }

        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
       
        public int Weightage { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

    

       
      
    }
}
