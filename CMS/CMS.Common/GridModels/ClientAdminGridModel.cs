using System;

namespace CMS.Common.GridModels
{
    public class ClientAdminGridModel
    {
        public int AId { get; set; }

        public int ClientId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string ContactNo { get; set; }

        public string ClientName { get; set; }

        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
