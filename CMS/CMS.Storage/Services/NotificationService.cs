using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class NotificationService : INotificationService
    {
        readonly IRepository _repository;
        public NotificationService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(Notification notification)
        {
            CMSResult result = new CMSResult();
            _repository.Add(notification);
            _repository.CommitChanges();
            result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Notification added successfully!") });
            return result;
        }

        public IEnumerable<NotificationGridModel> GetNotificationData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc,int ClientId)
        {
            var query = _repository.Project<Notification, IQueryable<NotificationGridModel>>(notifications => (
                 from n in notifications
                 where n.ClientId==ClientId
                 select new NotificationGridModel
                 {
                     NotificationId = n.NotificationId,
                     NotificationMessage = n.NotificationMessage,
                     AllUser = n.AllUser,
                     BranchAdminCount = n.BranchAdminCount,
                     TeacherCount = n.TeacherCount,
                     StudentCount = n.StudentCount,
                     ParentCount = n.ParentCount,
                     ClientAdminCount = n.ClientAdminCount,
                     Media = n.Media,
                     CreatedOn = n.CreatedOn
                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(NotificationGridModel.NotificationMessage):
                        if (!desc)
                            query = query.OrderBy(p => p.NotificationMessage);
                        else
                            query = query.OrderByDescending(p => p.NotificationMessage);
                        break;
                    case nameof(NotificationGridModel.AllUser):
                        if (!desc)
                            query = query.OrderBy(p => p.AllUser);
                        else
                            query = query.OrderByDescending(p => p.AllUser);
                        break;
                    case nameof(NotificationGridModel.BranchAdminCount):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchAdminCount);
                        else
                            query = query.OrderByDescending(p => p.BranchAdminCount);
                        break;
                    case nameof(NotificationGridModel.TeacherCount):
                        if (!desc)
                            query = query.OrderBy(p => p.TeacherCount);
                        else
                            query = query.OrderByDescending(p => p.TeacherCount);
                        break;
                    case nameof(NotificationGridModel.StudentCount):
                        if (!desc)
                            query = query.OrderBy(p => p.StudentCount);
                        else
                            query = query.OrderByDescending(p => p.StudentCount);
                        break;
                    case nameof(NotificationGridModel.ParentCount):
                        if (!desc)
                            query = query.OrderBy(p => p.ParentCount);
                        else
                            query = query.OrderByDescending(p => p.ParentCount);
                        break;
                    case nameof(NotificationGridModel.Media):
                        if (!desc)
                            query = query.OrderBy(p => p.Media);
                        else
                            query = query.OrderByDescending(p => p.Media);
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

        public NotificationProjection GetNotificationById(int id)
        {
            return _repository.Project<Notification, NotificationProjection>(
                Notification => (from n in Notification
                                 where n.NotificationId == id
                                 select new NotificationProjection
                                 {
                                     NotificationMessage = n.NotificationMessage,
                                     CreatedOn = n.CreatedOn,
                                     AllUser = n.AllUser,
                                     BranchAdminCount = n.BranchAdminCount,
                                     TeacherCount = n.TeacherCount,
                                     StudentCount = n.StudentCount,
                                     ParentCount = n.ParentCount,
                                     Media = n.Media,
                                     SelectedBranches = n.SelectedBranches,
                                     SelectedBatches = n.SelectedBatches,
                                     SelectedClasses = n.SelectedClasses,
                                     NotificationAutoDate=n.NotificationAutoDate
                                 }).FirstOrDefault());
        }

        public List<NotificationProjection> GetAutoNotificationsToSend(DateTime localDate)
        {
            
            return _repository.Project<Notification, List<NotificationProjection>>(
                Notification => (from n in Notification
                                 where n.NotificationAutoDate == localDate
                                 && n.IsSend == false
                                 select new NotificationProjection
                                 {
                                     NotificationMessage = n.NotificationMessage,
                                     CreatedOn = n.CreatedOn,
                                     AllUser = n.AllUser,
                                     BranchAdminCount = n.BranchAdminCount,
                                     TeacherCount = n.TeacherCount,
                                     StudentCount = n.StudentCount,
                                     ParentCount = n.ParentCount,
                                     Media = n.Media,
                                     SelectedBranches = n.SelectedBranches,
                                     SelectedBatches = n.SelectedBatches,
                                     SelectedClasses = n.SelectedClasses,
                                     UserName = n.UserName,
                                     NotificationId = n.NotificationId
                                 }).ToList());
        }

        public CMSResult Update(Notification oldNotification)
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();

            var notification = _repository.Load<Notification>(x => x.NotificationId == oldNotification.NotificationId);
            notification.IsSend = oldNotification.IsSend;

            _repository.Update(notification);
            result.IsSuccessful = true;
            result.Message = string.Format("Notification updated successfully!");

            cmsresult.Results.Add(result);
            return cmsresult;
        }
    }
}