using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common.GridModels
{
    public class PDFCategoryGridModel
    {
        public int PDFCategoryId { get; set; }

        public string Name { get; set; }

        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }
    }
}
