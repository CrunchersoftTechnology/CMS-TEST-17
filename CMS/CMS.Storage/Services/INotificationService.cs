using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface INotificationService
    {
        CMSResult Save(Notification notification);
        IEnumerable<NotificationGridModel> GetNotificationData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc, int ClientId);
        NotificationProjection GetNotificationById(int id);
        List<NotificationProjection> GetAutoNotificationsToSend(DateTime localDate);
        CMSResult Update(Notification oldNotification);
    }
}
