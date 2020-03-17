using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Projections
{
    public class ConfigureProjection
    {
        // public int AId { get; set; }
        public int ClientId { get; set; }

        public int ConfigureId { get; set; }

        public string name { get; set; }

        public string aboutus { get; set; }

        public string address { get; set; }

        public string email_id { get; set; }

        public string emailpassword { get; set; }

        public string sender_id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string websiteURL { get; set; }

        public string confirmemail { get; set; }

        //   public string brocherfile { get; set; }
    }
}
