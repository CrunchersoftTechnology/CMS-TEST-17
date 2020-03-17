using System;

namespace CMS.Domain.Storage.Services
{
    public interface IAspNetRoles
    {
        string GetCurrentUserRole(string roleUserId);
    }
}
