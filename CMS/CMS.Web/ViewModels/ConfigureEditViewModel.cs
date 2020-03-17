using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.Web.ViewModels
{
    public class ConfigureEditViewModel
    {
        public int ConfigureId { get; set; }
        public int ClientId { get; set; }
        // public string UserId { get; set; }

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

        [DisplayName("Email Password")]
        public string emailpassword { get; set; }

        [DisplayName("Sender Id")]
        public string sender_id { get; set; }

        [DisplayName("User Name")]
        public string username { get; set; }

        [DisplayName("Password")]
        public string password { get; set; }

        public string websiteURL { get; set; }

        public string confirmemail { get; set; }

        // public string CurrentUserRole { get; set; }
    }
}