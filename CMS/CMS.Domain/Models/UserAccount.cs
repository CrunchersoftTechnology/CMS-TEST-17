using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CMS.Domain.Models
{
    public class UserAccount
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is reqiured.")]
        [DisplayName("Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Mobile_No is reqiured.")]
        [DisplayName("Mobile No.")]
        public string mobile_no { get; set; }

        [Required(ErrorMessage = "Email_Id is reqiured.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [DisplayName("Email Id")]
        public string email_id { get; set; }

        [Required(ErrorMessage = "Password is reqiured.")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string password { get; set; }

        [Required(ErrorMessage = "Pin is reqiured.")]
        [DisplayName("Pin")]
        public string pin { get; set; }

        [Required(ErrorMessage = "Lincece Key is reqiured.")]
        [DisplayName("Lincence Key")]
        public string lincencekey { get; set; }


    }
}
