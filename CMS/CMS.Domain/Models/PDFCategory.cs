using CMS.Domain.Infrastructure;

namespace CMS.Domain.Models
{
    public class PDFCategory : AuditableEntity
    {

        public int ClientId { get; set; }
         
        public string ClientName { get; set; }
        public int PDFCategoryId { get; set; }

        public string Name { get; set; }
    }
}
