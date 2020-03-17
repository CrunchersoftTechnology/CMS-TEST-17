using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Projections
{
    public class SchoolProjection
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int SchoolId { get; set; }

        public string Name { get; set; }

        public string CenterNumber { get; set; }
    }
}
