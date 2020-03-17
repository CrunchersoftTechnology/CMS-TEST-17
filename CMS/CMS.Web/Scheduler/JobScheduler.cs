using CMS.Common;
using CMS.Web.Scheduler.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CMS.Web.Scheduler
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<AutoNotificationJob>()
                .WithIdentity("job1", "group1")
                .Build();

            int timeInterval = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.IntervalInHoursValue]);
            if (timeInterval < 1)
            {
                timeInterval = 1;
            }

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInHours(timeInterval)
            .RepeatForever())
            .Build();

             scheduler.ScheduleJob(job, trigger);
        }
    }
}