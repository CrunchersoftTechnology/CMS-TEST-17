using System.Linq;
using System;

namespace CMS.Domain.Storage.Services
{
    public class AspNetRoles : IAspNetRoles
    {
        public string GetCurrentUserRole(string roleUserId)
        {
            CMSDbContext context = new CMSDbContext();
            var user = context.Roles.Where(u => u.Users.Any(r => r.UserId == roleUserId)).ToList();
            var roles = user != null ? user.FirstOrDefault().Name : "";
            return roles;
        }
    }
}
