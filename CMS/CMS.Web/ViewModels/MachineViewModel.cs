using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.Web.ViewModels
{
    public class MachineViewModel
    {
        public int MachineId { get; set; }
        
        [Required]
        [MaxLength(50, ErrorMessage = "The field Machine Name must be a minimum length of '3' and maximum length of '50'.")]
        [MinLength(3, ErrorMessage = "The field Machine Name must be a minimum length of '3' and maximum length of '50'.")]
        [Display(Name="Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        [MaxLength(50, ErrorMessage = "The field Serial Number must be a minimum length of '5' and maximum length of '50'.")]
        [MinLength(5, ErrorMessage = "The field Serial Number must be a minimum length of '5' and maximum length of '50'.")]
        public string SerialNumber { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Display(Name="Branch Name")]
        public string BranchName { get; set; }

        [Display(Name = "Branch")]
        public IEnumerable<SelectListItem> Branches { get; set; }

        public string CurrentUserRole { get; set; }
    }
}