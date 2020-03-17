using CMS.Domain.Storage.Services;
using Quartz;
using System.Web.Mvc;

namespace CMS.Web.Scheduler.Jobs
{
    public class AttendenceNotifyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Logic to send attendece notification batchwise
            var batchService = DependencyResolver.Current.GetService<IBatchService>();
        }
    }
}