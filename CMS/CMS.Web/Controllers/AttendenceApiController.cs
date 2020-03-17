using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class AttendenceApiController : ApiController
    {
        readonly IStudentService _studentService;
        readonly IBatchService _batchService;
        readonly IAttendanceService _attendanceService;
        readonly ILogger _logger;
        readonly IMachineService _machineSevice;
        readonly ISendNotificationService _sendNotificationService;
        readonly ILocalDateTimeService _localDateTimeService;

        public AttendenceApiController(IStudentService studentService,
            IBatchService batchService,
            IAttendanceService attendanceService,
            ILogger logger,
            IMachineService machineService,
            ISendNotificationService sendNotificationService,
            ILocalDateTimeService localDateTimeService)
        {
            _studentService = studentService;
            _batchService = batchService;
            _attendanceService = attendanceService;
            _logger = logger;
            _machineSevice = machineService;
            _sendNotificationService = sendNotificationService;
            _localDateTimeService = localDateTimeService;
        }

        [Route("Api/AttendenceApi")]
        public HttpResponseMessage Post(MachineAttendence data)
        {
            var parentAppId = ConfigurationManager.AppSettings[Constants.ParentAppId];
            var parentRestAppId = ConfigurationManager.AppSettings[Constants.ParentRestAppId];
            var studentAppId = ConfigurationManager.AppSettings[Constants.ParentAppId];
            var studentRestAppId = ConfigurationManager.AppSettings[Constants.ParentRestAppId];

            var currentDateTime = _localDateTimeService.GetDateTime();
            try
            {
                _logger.Info("Sync Data started.");
                int branchId = _machineSevice.IsMachineExists(data.MachineSerial);
                if (branchId != 0)
                {
                    List<StudentAttendanceDetails> studentAttendanceList = new List<StudentAttendanceDetails>();

                    List<int> dataSId = new List<int>();
                    dataSId.AddRange(data.PunchDataList.Select(x => x.PunchId));
                    var getStudentDetailsList = _studentService.GetStudentDetailForAttendanceList(dataSId, branchId);

                    if (getStudentDetailsList.Count() != 0)
                    {
                        foreach (var attData in data.PunchDataList)
                        {
                            var id = attData.PunchId;

                            var punchDate = attData.PunchDateTime.Date;
                            var punchTime = attData.PunchDateTime.ToString(Constants.TimeFormat);
                            var getStudentDetails = getStudentDetailsList.Where(x => x.PunchId == attData.PunchId).FirstOrDefault();

                            if (getStudentDetails != null)
                            {
                                #region Rough
                                //var punchTime = Convert.ToDateTime(attData.PunchDateTime.ToString("hh:mm tt"));
                                //var punchDate = attData.PunchDateTime.Date;
                                //var inTiming = getStudentDetails.InTime.TimeOfDay;
                                //var beforeTime = inTiming.Add(TimeSpan.FromMinutes(-30));
                                //var outTiming = getStudentDetails.OutTime.TimeOfDay;
                                //var afterTime = outTiming.Add(TimeSpan.FromMinutes(30));
                                //if (TimeSpan.Compare(punchTime.TimeOfDay, beforeTime) == 1 && TimeSpan.Compare(punchTime.TimeOfDay, afterTime) == -1)
                                //{
                                //var batchId = 0;
                                //var batchInTime = new DateTime();
                                //var batchOutTime = new DateTime();
                                //batchId = getStudentDetails.BatchId;
                                //batchInTime = getStudentDetails.InTime;
                                //batchOutTime = getStudentDetails.OutTime;
                                //} 
                                #endregion

                                studentAttendanceList.Add(new StudentAttendanceDetails
                                {
                                    SId = getStudentDetails.SId,
                                    PunchId = getStudentDetails.PunchId,
                                    ClassId = getStudentDetails.ClassId,
                                    PunchDateTime = punchDate,
                                    BranchId = getStudentDetails.BranchId,
                                    BatchId = getStudentDetails.BatchId,
                                    Time = punchTime
                                });
                            }
                        }

                        #region backup for getting list
                        //var id = attData.PunchId;
                        //var getStudentDetails = _studentService.GetStudentDetailForAttendance(attData.PunchId, branchId);
                        //var batchId = 0;
                        //var batchInTime = new DateTime();
                        //var batchOutTime = new DateTime();

                        //if (getStudentDetails != null)
                        //{
                        //    foreach (var batch in getStudentDetails.Batches)
                        //    {
                        //        var punchTime = Convert.ToDateTime(attData.PunchDateTime.ToString("hh:mm tt"));
                        //        var punchDate = attData.PunchDateTime.Date;
                        //        if (TimeSpan.Compare(punchTime.TimeOfDay, batch.InTime.TimeOfDay) == 1 && TimeSpan.Compare(punchTime.TimeOfDay, batch.OutTime.TimeOfDay) == -1)
                        //        {
                        //            batchId = batch.BatchId;
                        //            batchInTime = batch.InTime;
                        //            batchOutTime = batch.OutTime;

                        //            studentAttendanceList.Add(new StudentAttendanceDetails
                        //            {
                        //                SId = getStudentDetails.SId,
                        //                PunchId = getStudentDetails.PunchId,
                        //                ClassId = getStudentDetails.ClassId,
                        //                PunchDateTime = punchDate,
                        //                BranchId = getStudentDetails.BranchId,
                        //                BatchId = batchId,
                        //                BatchInTime = batchInTime,
                        //                BatchOutTime = batchOutTime
                        //            });
                        //        }
                        //    }
                        //} 
                        #endregion
                    }

                    var punchesDateTimeWise = data.PunchDataList.OrderBy(x => x.PunchId).ToList();
                    var punches = data.PunchDataList.OrderBy(x => x.PunchDateTime).ToList();

                    if (studentAttendanceList.Count > 0)
                    {
                        var result = (from attendance in studentAttendanceList
                                      group attendance by new { attendance.ClassId, attendance.BatchId, attendance.PunchDateTime, attendance.BranchId } into grouping
                                      select new StudentAttendanceDetails
                                      {
                                          ClassId = grouping.Key.ClassId,
                                          BatchId = grouping.Key.BatchId,
                                          PunchDateTime = grouping.Key.PunchDateTime,
                                          BranchId = grouping.Key.BranchId,
                                          SelectedAttendance = string.Join(",", grouping.Select(x => x.SId).Distinct())
                                      });

                        var todayAttendanceList = studentAttendanceList.Where(x => x.PunchDateTime == currentDateTime.Date).ToList();

                        foreach (var item in result)
                        {
                            var projection = _attendanceService.GetExistingAttendance(item.ClassId, item.BatchId, item.PunchDateTime, item.BranchId);

                            if (projection != null)
                            {
                                #region UpdateAttendance
                                _logger.Info("Update section....");
                                var jsonObjectInTiming = new List<PunchDetails>();
                                var jsonObjectOutTiming = new List<PunchDetails>();

                                var inTiming = JsonConvert.DeserializeObject<List<PunchDetails>>(projection.InTime);
                                var outTiming = projection.OutTime != null ?
                                    JsonConvert.DeserializeObject<List<PunchDetails>>(projection.OutTime) : new List<PunchDetails>();

                                var oldAttendanceListDb = projection.StudentAttendence.Split(',')
                                                            .Where(x => !string.IsNullOrEmpty(x))
                                                            .Select(int.Parse).ToList();

                                var getAttendanceList = item.SelectedAttendance.Split(',')
                                                    .Where(x => !string.IsNullOrEmpty(x))
                                                    .Select(int.Parse).ToList();

                                _logger.Info(item.SelectedAttendance + " = SelectedAttendance");

                                List<int> finalRollNos = new List<int>();
                                finalRollNos.AddRange(oldAttendanceListDb);

                                foreach (var sId in getAttendanceList)
                                {
                                    var studentDetails = getStudentDetailsList.Where(x => x.SId == sId).FirstOrDefault();
                                    var punchInTime = punches.Where(x => x.PunchId == studentDetails.PunchId && x.PunchDateTime.Date == item.PunchDateTime.Date).FirstOrDefault().PunchDateTime.ToString(Constants.TimeFormat);
                                    if (!oldAttendanceListDb.Contains(sId))
                                    {
                                        finalRollNos.Add(sId);
                                        jsonObjectInTiming.Add(new PunchDetails
                                        {
                                            SId = studentDetails.SId,
                                            PunchTime = punchInTime
                                        });
                                    }
                                    else
                                    {
                                        var isExistsInLeave = outTiming.Where(x => x.SId == studentDetails.SId).FirstOrDefault();
                                        var isExistsInTime = inTiming.Where(x => x.SId == studentDetails.SId).FirstOrDefault();
                                        #region rough
                                        //if (oldAttendanceListDb.Contains(studentDetails.SId))
                                        //{
                                        //    isExistsInTime = inTiming.Where(x => x.SId == studentDetails.SId).FirstOrDefault();
                                        //}
                                        //if (finalRollNos.Contains(studentDetails.SId))
                                        //{
                                        //    isExistsInTime = jsonObjectInTiming.Where(x => x.SId == studentDetails.SId).FirstOrDefault();
                                        //} 
                                        #endregion

                                        if (isExistsInLeave == null)
                                        {
                                            var countOfPunches = punches.Where(x => x.PunchId == studentDetails.PunchId && x.PunchDateTime.Date == item.PunchDateTime.Date).ToList().Count();
                                            if (countOfPunches >= 1)
                                            {
                                                var punchOutTimeLists = punches.Where(x => x.PunchId == studentDetails.PunchId && x.PunchDateTime.Date == item.PunchDateTime.Date).ToList();
                                                foreach (var punchTime in punchOutTimeLists)
                                                {
                                                    string punchOutTime = "";
                                                    punchOutTime = punchTime.PunchDateTime.ToString(Constants.TimeFormat);

                                                    var duration = DateTime.Parse(punchOutTime) - DateTime.Parse(isExistsInTime.PunchTime);

                                                    _logger.Info(duration.ToString() + " duration");
                                                    if (duration.TotalHours >= 1)
                                                    {
                                                        _logger.Info(punchOutTime + " out time Added");
                                                        jsonObjectOutTiming.Add(new PunchDetails
                                                        {
                                                            SId = studentDetails.SId,
                                                            PunchTime = punchOutTime
                                                        });
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                _logger.Info("Update started....");
                                var sIdList = string.Join(",", finalRollNos);
                                if (jsonObjectInTiming.Count != 0)
                                    inTiming.AddRange(jsonObjectInTiming);
                                if (jsonObjectOutTiming.Count != 0)
                                    outTiming.AddRange(jsonObjectOutTiming);
                                var inTimeObject = JsonConvert.SerializeObject(inTiming);
                                var outTimeObject = outTiming != null ? JsonConvert.SerializeObject(outTiming) : null;

                                _logger.Info("Json serialize completed....");

                                var cmsResult = _attendanceService.UpdateAutoAttendance(new Attendance
                                {
                                    AttendanceId = projection.AttendanceId,
                                    ClassId = projection.ClassId,
                                    BatchId = projection.BatchId,
                                    UserId = projection.UserId,
                                    Activity = projection.Activity,
                                    Date = projection.Date,
                                    StudentAttendence = sIdList,
                                    BranchId = projection.BranchId,
                                    IsManual = false,
                                    InTime = inTimeObject,
                                    OutTime = outTimeObject
                                });
                                _logger.Info("Update Finished....");

                                #region SendAPPNotification
                                if (item.PunchDateTime == currentDateTime.Date)
                                {
                                    int i = 0;
                                    List<SendNotificationByPlayerId> appNotificationModelList = new List<SendNotificationByPlayerId>();
                                    foreach (var inTime in jsonObjectInTiming)
                                    {
                                        var student = getStudentDetailsList.Where(x => x.PunchId == inTime.SId).FirstOrDefault();
                                        var studentDetails = todayAttendanceList.Where(x => x.PunchId == student.PunchId).FirstOrDefault();
                                        var appNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Intime - {1}.", student.StudentName, inTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.studentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(appNotificationModel);
                                        var parentAppNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Intime - {1}.", student.StudentName, inTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.parentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(parentAppNotificationModel);
                                    }
                                    foreach (var outTime in jsonObjectOutTiming)
                                    {
                                        var student = getStudentDetailsList.Where(x => x.PunchId == outTime.SId).FirstOrDefault();
                                        var studentDetails = todayAttendanceList.Where(x => x.PunchId == student.PunchId).FirstOrDefault();
                                        var appNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Outtime - {1}.", student.StudentName, outTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.studentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(appNotificationModel);
                                        var parentAppNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Outtime - {1}.", student.StudentName, outTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.parentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(parentAppNotificationModel);
                                    }

                                    _logger.Info(i + " exists sms");
                                    if (jsonObjectInTiming.Count > 0 || jsonObjectOutTiming.Count > 0)
                                    {
                                        var appNotificationModelArray = appNotificationModelList.ToArray();
                                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(appNotificationModelArray, cancellationToken));
                                    }
                                }
                                #endregion

                                #endregion
                            }
                            else
                            {
                                #region Save Attendance
                                _logger.Info("Save section....");
                                var jsonObjectInTiming = new List<PunchDetails>();
                                var jsonObjectOutTiming = new List<PunchDetails>();

                                var sIdList = item.SelectedAttendance;

                                var finalRollNos = item.SelectedAttendance.Split(',')
                                                        .Where(x => !string.IsNullOrEmpty(x))
                                                        .Select(int.Parse).ToList();

                                foreach (var rollNo in finalRollNos)
                                {
                                    var studentDetailss = studentAttendanceList.Where(x => x.SId == rollNo).FirstOrDefault();
                                    var punchInTime = punches.Where(x => x.PunchId == studentDetailss.PunchId && x.PunchDateTime.Date == item.PunchDateTime.Date).FirstOrDefault().PunchDateTime.ToString(Constants.TimeFormat);

                                    jsonObjectInTiming.Add(new PunchDetails
                                    {
                                        SId = studentDetailss.SId,
                                        PunchTime = punchInTime
                                    });

                                    var countOfPunches = punches.Where(x => x.PunchId == studentDetailss.PunchId && x.PunchDateTime.Date == item.PunchDateTime.Date).ToList().Count();
                                    if (countOfPunches >= 1)
                                    {
                                        var punchOutTimeLists = punches.Where(x => x.PunchId == studentDetailss.PunchId && x.PunchDateTime.Date == item.PunchDateTime.Date).ToList();
                                        foreach (var punchTime in punchOutTimeLists)
                                        {
                                            string punchOutTime = "";
                                            punchOutTime = punchTime.PunchDateTime.ToString(Constants.TimeFormat);

                                            var duration = DateTime.Parse(punchOutTime) - DateTime.Parse(punchInTime);
                                            if (duration.TotalHours >= 1)
                                            {
                                                jsonObjectOutTiming.Add(new PunchDetails
                                                {
                                                    SId = studentDetailss.SId,
                                                    PunchTime = punchOutTime
                                                });
                                                break;
                                            }
                                        }
                                    }
                                }

                                _logger.Info("Save started....");
                                var inTimeObject = JsonConvert.SerializeObject(jsonObjectInTiming);
                                var outTimeObject = jsonObjectOutTiming.Count != 0 ? JsonConvert.SerializeObject(jsonObjectOutTiming) : null;
                                _logger.Info("Json serialize completed....");
                                //var sIdList = string.Join(",", studentAttendance);
                                var cmsResult = _attendanceService.Save(new Attendance
                                {
                                    ClassId = item.ClassId,
                                    BatchId = item.BatchId,
                                    Activity = "NA",
                                    StudentAttendence = sIdList,
                                    BranchId = item.BranchId,
                                    Date = item.PunchDateTime,
                                    IsManual = false,
                                    InTime = inTimeObject,
                                    OutTime = outTimeObject
                                });
                                _logger.Info("Save Finished....");

                                #region SendAPPNotification
                                if (item.PunchDateTime == currentDateTime.Date)
                                {
                                    int i = 0;
                                    List<SendNotificationByPlayerId> appNotificationModelList = new List<SendNotificationByPlayerId>();
                                    foreach (var inTime in jsonObjectInTiming)
                                    {
                                        var student = getStudentDetailsList.Where(x => x.PunchId == inTime.SId).FirstOrDefault();
                                        var studentDetails = todayAttendanceList.Where(x => x.PunchId == student.PunchId).FirstOrDefault();
                                        var appNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Intime - {1}.", student.StudentName, inTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.studentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(appNotificationModel);
                                        var parentAppNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Intime - {1}.", student.StudentName, inTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.parentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(parentAppNotificationModel);
                                    }
                                    foreach (var outTime in jsonObjectOutTiming)
                                    {
                                        var student = getStudentDetailsList.Where(x => x.PunchId == outTime.SId).FirstOrDefault();
                                        var studentDetails = todayAttendanceList.Where(x => x.PunchId == student.PunchId).FirstOrDefault();
                                        var appNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Outtime - {1}.", student.StudentName, outTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.studentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(appNotificationModel);
                                        var parentAppNotificationModel = new SendNotificationByPlayerId
                                        {
                                            Message = string.Format("Today {0} is Present. Outtime - {1}.", student.StudentName, outTime.PunchTime),
                                            AppIds = parentAppId,
                                            RestApiKey = parentRestAppId,
                                            PlayerIds = student.parentAppPlayerId,
                                        };
                                        appNotificationModelList.Add(parentAppNotificationModel);
                                    }

                                    _logger.Info(i + " exists sms");
                                    if (jsonObjectInTiming.Count > 0 || jsonObjectOutTiming.Count > 0)
                                    {
                                        var appNotificationModelArray = appNotificationModelList.ToArray();
                                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(appNotificationModelArray, cancellationToken));
                                    }
                                }
                                #endregion

                                #endregion
                            }
                        }
                    }
                }
                else
                {
                    _logger.Error("Machine Serial does not matched..");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString() + "attendance api");
            }
            _logger.Info("Sync Successfully to server.");
            return Request.CreateResponse(HttpStatusCode.OK, "Post Successful!");
        }

        [Route("Api/AttendenceApi/Get")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Get Successful!");
        }

        [Route("Api/AttendenceApi/StudetnList")]
        public HttpResponseMessage StudetnList(MachineAttendence data)
        {
            List<int> attendanceId = new List<int>();
            List<StudentDetails> studentAttendanceList = new List<StudentDetails>();
            try
            {
                int branchId = _machineSevice.IsMachineExists(data.MachineSerial);
                if (branchId != 0)
                {
                    var attendances = _attendanceService.GetAttendanceToSendNotification(branchId, DateTime.Now);

                    foreach (var attendance in attendances)
                    {
                        var students = _studentService.GetStudentsByBranchAndClassIdForAttendance(attendance.ClassId, branchId, attendance.Date);
                        foreach (var student in students)
                        {
                            if (attendance.BatchId == student.BatchId)
                            {
                                var sid = attendance.StudentAttendence.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

                                if (student.parentAppPlayerId != null)
                                {
                                    if (sid.Contains(student.SId))
                                    {
                                        studentAttendanceList.Add(new StudentDetails
                                        {
                                            SId = student.SId,
                                            ParentAppPlayerId = student.parentAppPlayerId,
                                            date = attendance.Date,
                                            batch = student.BatchName,
                                            status = "Present"
                                        });
                                    }
                                    else
                                    {
                                        studentAttendanceList.Add(new StudentDetails
                                        {
                                            SId = student.SId,
                                            ParentAppPlayerId = student.parentAppPlayerId,
                                            date = attendance.Date,
                                            batch = student.BatchName,
                                            status = "Absent"
                                        });
                                    }
                                }
                            }
                        }
                        attendanceId.AddRange(attendances.Select(x => x.AttendanceId));
                    }
                }
                List<string> listOfPlayerId = new List<string>();
                var i = 0;
                SendNotificationByPlayerId[] notification = new SendNotificationByPlayerId[studentAttendanceList.Count()];
                foreach (var appNotification in studentAttendanceList)
                {
                    listOfPlayerId.Add(appNotification.ParentAppPlayerId);
                    if (!(appNotification.ParentAppPlayerId == null || appNotification.ParentAppPlayerId == ""))
                    {
                        var sendAppNotification = new SendNotificationByPlayerId
                        {
                            Message = "Attendance-" + appNotification.status + "Date : " + appNotification.date + "Batch : " + appNotification.batch,
                            PlayerIds = appNotification.ParentAppPlayerId,
                            AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                            RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                        };
                        notification[i] = sendAppNotification;
                        i++;
                    }
                }
                if (listOfPlayerId.Count > 0)
                {
                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
                    var result = _attendanceService.UpdateMultipleAttendance(string.Join(",", attendanceId));
                    _logger.Info("Send Notification Successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString() + "StudetnList");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "send notification successfully.");
        }

        public class StudentDetails
        {
            public int SId { get; set; }
            public string Email { get; set; }
            public string ParentAppPlayerId { get; set; }
            public DateTime date { get; set; }
            public string batch { get; set; }
            public string status { get; set; }
        }

        public class PunchDetails
        {
            public int SId { get; set; }
            public string PunchTime { get; set; }
        }
    }
}
