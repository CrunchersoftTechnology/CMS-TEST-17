using CMS.Common;
using CMS.Domain.Models;

namespace CMS.Domain.Storage.Services
{
    public interface IApplicationUserService
    {
        CMSResult Save(ApplicationUser user, string password);
        CMSResult Update(ApplicationUser user);
        CMSResult SaveTeacher(ApplicationUser user, string password);
        CMSResult SaveBranchAdmin(ApplicationUser user, string password);
        CMSResult SaveClientAdmin(ApplicationUser user, string password);
    }
}
