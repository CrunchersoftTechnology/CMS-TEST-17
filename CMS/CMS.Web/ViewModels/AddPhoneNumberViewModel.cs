using System.ComponentModel.DataAnnotations;

namespace CMS.Web.ViewModels
{

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}