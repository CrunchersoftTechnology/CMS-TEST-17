namespace CMS.Domain.Storage.Projections
{
    public class PDFCategoryProjection
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }
        public int PDFCategoryId { get; set; }
        
        public string Name { get; set; }

        public string DelimitedPdfIds { get; set; }
    }
}
