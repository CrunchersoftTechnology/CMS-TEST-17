using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.GridModels
{
   public class ChapterGridModel
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public int SubjectId { get; set; }
        public int Weightage { get; set; }
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
