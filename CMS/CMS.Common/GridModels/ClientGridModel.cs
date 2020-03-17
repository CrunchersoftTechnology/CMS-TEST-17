using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common.GridModels
{
    public class ClientGridModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }

        [Exclude] //Exclude column from export 
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }


    }
}
