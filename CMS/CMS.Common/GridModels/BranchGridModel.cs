using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common.GridModels
{
    public class BranchGridModel
    {

        public string UserId { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public string Address { get; set; }

        [Exclude] //Exclude column from export 
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }

    }
}
