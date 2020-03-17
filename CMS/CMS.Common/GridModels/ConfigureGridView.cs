using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

namespace CMS.Common.GridModels
{
    public class ConfigureGridModel
    {
        // public string UserId { get; set; }

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

        [Exclude] //Exclude column from export 
        public string Action { get; set; }

        //  public string brocherfile { get; set; }
        public string websiteURL { get; set; }

        public string confirmemail { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
