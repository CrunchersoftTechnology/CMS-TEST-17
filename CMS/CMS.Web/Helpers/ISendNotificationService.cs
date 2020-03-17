using CMS.Common;
using CMS.Web.Models;
using System.Threading;

namespace CMS.Web.Helpers
{
    public interface ISendNotificationService
    {
        CMSResult SendNotification(SendNotification model);
        CMSResult SendNotificationByPlayersId(SendNotification model);
      //  void StartProcessing(SendNotification[] notificationModels, CancellationToken cancellationToken = default(CancellationToken));
        CMSResult SendNotificationSingle(SendNotificationByPlayerId model);
        void StartProcessingByPlayerId(SendNotificationByPlayerId[] notificationModels, CancellationToken cancellationToken = default(CancellationToken));
    }
}
