using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web;

namespace CMS.Domain.Models
{
    public class Configure : AuditableEntity
    {
        public int ConfigureId { get; set; }
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Name is reqiured.")]
        [DisplayName("Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Aboutus is reqiured.")]
        [DisplayName("Aboutus")]
        public string aboutus { get; set; }

        [DisplayName("Address")]
        public string address { get; set; }

        [Required(ErrorMessage = "Email_Id is reqiured.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [DisplayName("Email Id")]
        public string email_id { get; set; }

        [DisplayName("EmailPassword")]
        public string emailpassword { get; set; }

        [DisplayName("Sender Id")]
        public string sender_id { get; set; }

        [DisplayName("User Name")]
        public string username { get; set; }

        [DisplayName("Password")]
        public string password { get; set; }

        [DisplayName("Class Website URL")]
        public string websiteURL { get; set; }

        [DisplayName("Confirm Email")]
        public string confirmemail { get; set; }

        //public HttpPostedFileBase Pdffile { get; set; }

    }
}
