using CMS.Web.Models;
using System.Collections.Generic;
using System.Threading;

namespace CMS.Web.Helpers
{
    public interface IEmailService
    {
        bool MailSend(MailModel model);
        bool MailSend(MailModel model, List<string> multipleRecipients);
        bool Send(MailModel model);
        bool Send(MailModel model, List<string> multipleRecipients);
        void StartProcessing(MailModel[] mailModels, CancellationToken cancellationToken = default(CancellationToken));
    }
}