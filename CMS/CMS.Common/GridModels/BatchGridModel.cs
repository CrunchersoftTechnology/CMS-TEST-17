using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.GridModels
{
   public class BatchGridModel
    {

        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string SubjectName { get; set; }
        public int SubjectId { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public string ClassName { get; set; }
        public int ClassId { get; set; }
        [Exclude]
        public string Action { get; set; }
        public DateTime CreatedOn { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }
    }
}
