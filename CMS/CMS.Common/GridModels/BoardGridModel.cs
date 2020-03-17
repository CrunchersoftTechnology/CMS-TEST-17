using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.GridModels
{
    public class BoardGridModel
    {
        public string UserId { get; set; }

        public int BoardId { get; set; }

        public string BoardName { get; set; }

        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }
    }
}
