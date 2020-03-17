using CMS.Common;
using CMS.Web.Logger;
using CMS.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace CMS.Web.Helpers
{
    public class SendNotificationService : ISendNotificationService
    {
        readonly ILogger _logger;

        public SendNotificationService(ILogger logger)
        {
            _logger = logger;
        }

        public CMSResult SendNotification(SendNotification model)
        {
            var cmsResult = new CMSResult();
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            var authorizationValue = string.Format("Basic {0}", model.RestApiKey);
            request.Headers.Add("authorization", authorizationValue);
            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = model.AppIds,
                contents = new { en = model.Message },
                included_segments = new string[] { "All" },
                content_available = true,
                priority = 10,
                isInAppFocus = true
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);
            string responseContent = null;
            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                _logger.Error(ex.Message + "SendNotification");
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                return cmsResult;
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            var jsonResponseObject = (JObject)JsonConvert.DeserializeObject<dynamic>(responseContent);
            var errorData = (jsonResponseObject["errors"]);
            if (errorData == null)
            {
                cmsResult.Results.Add(new Result { Message = "App notification sent successfully.", IsSuccessful = true });
                return cmsResult;
            }
            cmsResult.Results.Add(new Result { Message = "Error occured.", IsSuccessful = false });
            return cmsResult;
        }

        public CMSResult SendNotificationByPlayersId(SendNotification model)
        {
            var cmsResult = new CMSResult();
            var authorizationValue = string.Format("Basic {0}", model.RestApiKey);
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", authorizationValue);
            var serializer = new JavaScriptSerializer();

            var obj = new
            {
                app_id = model.AppIds,
                contents = new { en = model.Message },
                include_player_ids = model.PlayerIds,
                content_available = 1,
                priority = 10
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);
            string responseContent = null;
            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                _logger.Error(ex.Message + "SendNotificationByPlayersId");
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                return cmsResult;
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            var jsonResponseObject = (JObject)JsonConvert.DeserializeObject<dynamic>(responseContent);
            var errorData = (jsonResponseObject["errors"]);
            if (errorData == null)
            {
                cmsResult.Results.Add(new Result { Message = "App notification sent successfully.", IsSuccessful = true });
                return cmsResult;
            }
            cmsResult.Results.Add(new Result { Message = "Error occured.", IsSuccessful = false });
            return cmsResult;
        }

        public CMSResult SendNotificationSingle(SendNotificationByPlayerId model)
        {
            var cmsResult = new CMSResult();
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            var authorizationValue = string.Format("Basic {0}", model.RestApiKey);
            request.Headers.Add("authorization", authorizationValue);
            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = model.AppIds,
                contents = new { en = model.Message },
                include_player_ids = new string[] { model.PlayerIds },
                content_available = 1,
                priority = 10
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);
            string responseContent = null;
            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                _logger.Error(ex.Message + "SendNotificationSingle");
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                return cmsResult;
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            var jsonResponseObject = (JObject)JsonConvert.DeserializeObject<dynamic>(responseContent);
            var errorData = (jsonResponseObject["errors"]);
            if (errorData == null)
            {
                cmsResult.Results.Add(new Result { Message = "App notification sent successfully.", IsSuccessful = true });
                return cmsResult;
            }
            cmsResult.Results.Add(new Result { Message = "Error occured.", IsSuccessful = false });
            return cmsResult;
        }

        public void StartProcessingByPlayerId(SendNotificationByPlayerId[] notificationModels, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                {
                    foreach (var notificationModel in notificationModels)
                    {
                        //execute when task has been cancel  
                        cancellationToken.ThrowIfCancellationRequested();
                        //send email here
                        SendNotificationSingle(notificationModel);
                        _logger.Info("App Notification Send Successfully");
                        Thread.Sleep(1500);   // wait to 1.5 sec every time  
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }

    }
}