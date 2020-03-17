using CMS.Common;
using CMS.Web.Models;
using System.Threading;

namespace CMS.Web.Helpers
{
    public interface ISmsService
    {
        CMSResult DynamicsSendMessage(SmsModel model);
        CMSResult SendMessage(SmsModel model);        
        string CreditCheck();
        void StartProcessing(SmsModel[] smsModels, CancellationToken cancellationToken = default(CancellationToken));
    }
}
