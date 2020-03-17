using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common.GridModels
{
   public class SubjectGridModel
    {
        public string UserId { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

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
