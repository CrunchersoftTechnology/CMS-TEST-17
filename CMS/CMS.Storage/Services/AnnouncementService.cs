using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        readonly IRepository _repository;

        public AnnouncementService(IRepository repository)
        {
            _repository = repository;
        }

        public bool Delete(int id)
        {
            var announcement = _repository.Load<Announcement>(x => x.AnnouncementId == id);
            if (announcement == null)
                return false;
            _repository.Delete(announcement);
            return true;
        }

        public IEnumerable<AnnouncementProjection> GetAllAnnouncements(int limit)
        {
            if (limit == 0)
            {
                return _repository.Project<Announcement, AnnouncementProjection[]>(ans => (from a in ans
                                                                                           orderby a.UpdatedOn.HasValue descending
                                                                                           orderby a.CreatedOn descending
                                                                                           select new AnnouncementProjection
                                                                                           {
                                                                                               AnnouncementDetails = a.AnnouncementDetails,
                                                                                               AnnouncementId = a.AnnouncementId,
                                                                                               CreatedOn = a.CreatedOn,
                                                                                               UpdatedOn = a.UpdatedOn,
                                                                                               IsVisible = a.IsVisible,
                                                                                               Url = a.Url
                                                                                           }).ToArray());
            }
            else
            {
                return _repository.Project<Announcement, AnnouncementProjection[]>(ans => (from a in ans
                                                                                           orderby a.UpdatedOn.HasValue descending
                                                                                           orderby a.CreatedOn descending
                                                                                           where a.IsVisible
                                                                                           select new AnnouncementProjection
                                                                                           {
                                                                                               AnnouncementDetails = a.AnnouncementDetails,
                                                                                               AnnouncementId = a.AnnouncementId,
                                                                                               CreatedOn = a.CreatedOn,
                                                                                               UpdatedOn = a.UpdatedOn,
                                                                                               IsVisible = a.IsVisible,
                                                                                               Url = a.Url
                                                                                           }).ToArray()).Take(limit);
            }
        }

        public Announcement GetAnnouncement(int id)
        {
            return _repository.Project<Announcement, Announcement>(list
                => (from a in list
                    where a.AnnouncementId == id
                    select a).FirstOrDefault());
        }

        public bool Save(Announcement announcement)
        {
            var result = _repository.Add(announcement);
            return true;
        }

        public bool Update(Announcement announcement)
        {
            var result = _repository.Load<Announcement>(x => x.AnnouncementId == announcement.AnnouncementId);
            if (result == null)
                return false;
            result.AnnouncementDetails = announcement.AnnouncementDetails;
            result.Url = announcement.Url;
            result.IsVisible = announcement.IsVisible;
            _repository.Update(result);
            return true;
        }

        public IEnumerable<AnnouncementGridModel> GetAnnouncementData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Announcement, IQueryable<AnnouncementGridModel>>(machines => (
                 from m in machines
                 select new AnnouncementGridModel
                 {
                     AnnouncementId = m.AnnouncementId,
                     AnnouncementDetails = m.AnnouncementDetails,
                     Url = m.Url,
                     IsVisible = m.IsVisible,
                     CreatedBy = m.CreatedBy,
                     CreatedOn = m.CreatedOn,
                     UpdatedBy = m.UpdatedBy,
                     UpdatedOn = m.UpdatedOn
                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(AnnouncementGridModel.AnnouncementDetails):
                        if (!desc)
                            query = query.OrderBy(p => p.AnnouncementDetails);
                        else
                            query = query.OrderByDescending(p => p.AnnouncementDetails);
                        break;
                    case nameof(AnnouncementGridModel.Url):
                        if (!desc)
                            query = query.OrderBy(p => p.Url);
                        else
                            query = query.OrderByDescending(p => p.Url);
                        break;
                    case nameof(AnnouncementGridModel.IsVisible):
                        if (!desc)
                            query = query.OrderBy(p => p.IsVisible);
                        else
                            query = query.OrderByDescending(p => p.IsVisible);
                        break;
                    case nameof(AnnouncementGridModel.CreatedOn):
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;

                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }


            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }


            return query.ToList();
        }
    }
}
