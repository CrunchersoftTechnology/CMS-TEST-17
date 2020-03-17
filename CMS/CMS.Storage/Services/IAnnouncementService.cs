using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IAnnouncementService
    {
        IEnumerable<AnnouncementProjection> GetAllAnnouncements(int limit);
        bool Save(Announcement announcement);
        bool Update(Announcement announcement);
        Announcement GetAnnouncement(int id);
        bool Delete(int id);
        IEnumerable<AnnouncementGridModel> GetAnnouncementData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }
}