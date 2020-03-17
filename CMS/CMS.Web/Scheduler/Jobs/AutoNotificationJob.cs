using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CMS.Web.Scheduler.Jobs
{
    public class AutoNotificationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();

            logger.Info("auto notification start");

            //Logic to send attendece notification batchwise
            var branchAdminService = DependencyResolver.Current.GetService<IBranchAdminService>();
            var clientAdminService = DependencyResolver.Current.GetService<IClientAdminService>();
            var studentService = DependencyResolver.Current.GetService<IStudentService>();
            var teacherService = DependencyResolver.Current.GetService<ITeacherService>();
            var repository = DependencyResolver.Current.GetService<IRepository>();
            var notificationService = DependencyResolver.Current.GetService<INotificationService>();
            var localDateTime = DependencyResolver.Current.GetService<ILocalDateTimeService>();

            var localDate = (localDateTime.GetDateTime()).Date;
            var notificationsToSend = notificationService.GetAutoNotificationsToSend(localDate);
            logger.Info(localDate.ToString() + " notificationsToSend count   " + notificationsToSend.Count());
            foreach (var notificationAuto in notificationsToSend)
            {
                var notificationMedia = notificationAuto.Media.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

                List<string> listOfNumber = new List<string>();
                List<string> listOfEmail = new List<string>();
                List<string> listOfPlayerId = new List<string>();
                List<string> listOfName = new List<string>();
                var finalPlayerId = new List<ListOfPlayerId>();

                var studentParentNoList = studentService.GetAllStudentParentList();
                var branchAdminContactList = branchAdminService.GetBranchAdminContactList();
                var clientAdminContactList = clientAdminService.GetClientAdminContactList();
                var teacherContactList = teacherService.GetTeacherContactList();

                if (!notificationAuto.AllUser)
                {
                    var branchIds = notificationAuto.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                    var clientIds = notificationAuto.SelectedClients.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                    if ((notificationAuto.StudentCount != 0) || (notificationAuto.ParentCount != 0))
                    {
                        var classIds = notificationAuto.SelectedClasses.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

                        var batchIds = notificationAuto.SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                        var b = repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId)).ToList();

                        studentParentNoList = studentParentNoList.Where(
                                                                 x => branchIds.Contains(x.BranchId) && clientIds.Contains(x.ClassId) &&
                                                                 batchIds.Contains(x.BatchId) &&
                                                                 classIds.Contains(x.ClassId));


                        foreach (var student in studentParentNoList)
                        {
                            bool isExists = true;

                            if (isExists)
                            {
                                if (notificationAuto.StudentCount != 0)
                                {
                                    listOfEmail.Add(student.Email);
                                    listOfNumber.Add(student.StudentContact);
                                    listOfName.Add(student.Name);
                                }
                                if (notificationAuto.ParentCount != 0)
                                {
                                    if (student.parentAppPlayerId != null && student.parentAppPlayerId != "")
                                    {
                                        var details = new ListOfPlayerId
                                        {
                                            SId = student.SId,
                                            ParentPlayerId = student.parentAppPlayerId
                                        };
                                        finalPlayerId.Add(details);
                                    }
                                    listOfEmail.Add(student.ParentEmailId);
                                    listOfNumber.Add(student.ParentContact);
                                    listOfName.Add(student.MiddleName + " " + student.LastName);
                                    listOfPlayerId.Add(student.parentAppPlayerId);
                                }
                            }
                        }
                    }
                    if (notificationAuto.BranchAdminCount != 0)
                    {
                        branchAdminContactList = branchAdminContactList.Where(x => branchIds.Contains(x.BranchId));
                        listOfEmail.AddRange(branchAdminContactList.Select(x => x.Email));
                        listOfNumber.AddRange(branchAdminContactList.Select(x => x.ContactNo));
                        listOfName.AddRange(branchAdminContactList.Select(x => x.Name));
                    }

                    if (notificationAuto.ClientAdminCount != 0)
                    {
                        clientAdminContactList = clientAdminContactList.Where(x => clientIds.Contains(x.ClientId));
                        listOfEmail.AddRange(clientAdminContactList.Select(x => x.Email));
                        listOfNumber.AddRange(clientAdminContactList.Select(x => x.ContactNo));
                        listOfName.AddRange(clientAdminContactList.Select(x => x.Name));
                    }
                    if (notificationAuto.TeacherCount != 0)
                    {
                        teacherContactList = teacherContactList.Where(x => branchIds.Contains(x.BranchId));
                        listOfEmail.AddRange(teacherContactList.Select(x => x.Email));
                        listOfNumber.AddRange(teacherContactList.Select(x => x.ContactNo));
                        listOfName.AddRange(teacherContactList.Select(x => x.Name));
                    }
                }
                else
                {
                    listOfEmail.AddRange(branchAdminContactList.Select(x => x.Email));

                    listOfEmail.AddRange(studentParentNoList.Select(x => x.Email));

                    listOfEmail.AddRange(teacherContactList.Select(x => x.Email));

                    listOfEmail.AddRange(clientAdminContactList.Select(x => x.Email));

                    listOfEmail.AddRange(studentParentNoList.Select(x => x.ParentEmailId));

                    listOfNumber.AddRange(branchAdminContactList.Select(x => x.ContactNo));

                    listOfNumber.AddRange(clientAdminContactList.Select(x => x.ContactNo));

                    listOfNumber.AddRange(studentParentNoList.Select(x => x.StudentContact));

                    listOfNumber.AddRange(teacherContactList.Select(x => x.ContactNo));

                    listOfNumber.AddRange(studentParentNoList.Select(x => x.ParentContact));

                    listOfPlayerId.AddRange(studentParentNoList.Select(x => x.parentAppPlayerId));
                    foreach (var item in studentParentNoList)
                    {
                        if (item.parentAppPlayerId != null && item.parentAppPlayerId != "")
                        {
                            var details = new ListOfPlayerId
                            {
                                SId = item.SId,
                                ParentPlayerId = item.parentAppPlayerId
                            };
                            finalPlayerId.Add(details);
                        }
                    }
                    listOfName.AddRange(branchAdminContactList.Select(x => x.Name));
                    listOfName.AddRange(clientAdminContactList.Select(x => x.Name));
                    listOfName.AddRange(studentParentNoList.Select(x => x.Name));
                    listOfName.AddRange(teacherContactList.Select(x => x.Name));
                    listOfName.AddRange(studentParentNoList.Select(x => x.MiddleName + " " + x.LastName));
                }

                if (notificationMedia.Contains("Email"))
                {
                    if (listOfEmail.Count > 0)
                    {
                        var result = SendEmail(notificationAuto.NotificationMessage, listOfEmail, listOfName, notificationAuto.UserName);

                        logger.Info(listOfEmail.Where(x => x != "").Count().ToString() + " email " + result);
                    }
                }

                if (notificationMedia.Contains("SMS"))
                {
                    if (listOfNumber.Count > 0)
                    {
                        var result = SendSMS(notificationAuto.NotificationMessage, listOfNumber);
                        logger.Info(listOfNumber.Count().ToString() + " sms " + result);
                    }
                }
                if (notificationMedia.Contains("AppNotification") && ((notificationAuto.StudentCount != 0) || (notificationAuto.ParentCount != 0) || notificationAuto.AllUser))
                {
                    List<string> savePlayerId = new List<string>();
                    savePlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                }

                try
                {
                    var notification = new Notification
                    {
                        IsSend = true,
                        NotificationId = notificationAuto.NotificationId
                    };
                    var result = notificationService.Update(notification);
                    logger.Info(listOfPlayerId.Count().ToString() + " notification save   " +DateTime.Now );
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + " catch auto notification");
                    throw;
                }

                if (notificationMedia.Contains("AppNotification") && ((notificationAuto.StudentCount != 0) || (notificationAuto.ParentCount != 0) || notificationAuto.AllUser))
                {
                    var response = SendAppNotification(notificationAuto, listOfPlayerId, finalPlayerId, notificationAuto.NotificationId);

                    logger.Info(listOfPlayerId.Count().ToString() + " app notification " + response);
                }
            }

        }

        public bool SendEmail(string notificationTextMessage, List<string> listOfEmail, List<string> listOfName, string userRole)
        {
            listOfEmail = listOfEmail.Where(x => x != "").ToList();
            var emailService = DependencyResolver.Current.GetService<IEmailService>();
            var i = 0;
            MailModel[] emailAddress = new MailModel[listOfEmail.Count];
            foreach (var emailId in listOfEmail)
            {
                if (listOfEmail[i] != "")
                {
                    string body = string.Empty;
                    var path = HostingEnvironment.MapPath("~/MailDesign/NotificationMailDesign.html");  
                    using (StreamReader reader = new StreamReader(path))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{BranchName}", userRole);
                    body = body.Replace("{NotificationMessage}", notificationTextMessage);
                    body = body.Replace("{UserName}", listOfName[i]);

                    var emailMessage = new MailModel
                    {
                        Body = body,
                        Subject = "Notice",
                        To = listOfEmail[i]
                    };
                    emailAddress[i] = emailMessage;
                }
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => emailService.StartProcessing(emailAddress, cancellationToken));
            return true;
        }

        public bool SendSMS(string notificationTextMessage, List<string> listOfNumber)
        {
            var smsService = DependencyResolver.Current.GetService<ISmsService>();
            var listOfContact = string.Join(",", listOfNumber);
            var smsModel = new SmsModel
            {
                Message = notificationTextMessage,
                SendTo = listOfContact
            };
            smsService.SendMessage(smsModel);
            return true;
        }

        public bool SendAppNotification(NotificationProjection viewModel, List<string> listOfPlayerId, List<ListOfPlayerId> finalPlayerId, int savedNotificationId)
        {
            var sendNotificationService = DependencyResolver.Current.GetService<ISendNotificationService>();
            var getfinalPlayerId = (from list in finalPlayerId
                                    group finalPlayerId by new
                                    {
                                        list.SId,
                                        list.ParentPlayerId
                                    } into grouping
                                    select new ListOfPlayerId
                                    {
                                        SId = grouping.Key.SId,
                                        ParentPlayerId = grouping.Key.ParentPlayerId
                                    }).ToList();
            listOfPlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (listOfPlayerId.Count > 0)
            {
                var i = 0;
                SendNotificationByPlayerId[] notification = new SendNotificationByPlayerId[getfinalPlayerId.Count()];
                foreach (var playerId in getfinalPlayerId)
                {
                    var studentSId = playerId.SId;
                    var sendAppNotification = new SendNotificationByPlayerId
                    {
                        Message = "Notice-" + viewModel.NotificationMessage + "$^$" + playerId.SId + "@" + savedNotificationId,
                        PlayerIds = playerId.ParentPlayerId,
                        AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                        RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                    };
                    notification[i] = sendAppNotification;
                    i++;
                }

                if (viewModel.AllUser)
                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
                else
                {
                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
                }
            }
            return true;
        }


        public class ListOfPlayerId
        {
            public int SId { get; set; }
            public string ParentPlayerId { get; set; }
        }
    }
}
