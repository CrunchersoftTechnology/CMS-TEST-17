using CMS.Common;
using CMS.Web.Logger;
using CMS.Web.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace CMS.Web.Helpers
{
    public class SmsService : ISmsService
    {
        string constr = ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString;

        readonly ILogger _logger;

        public SmsService(ILogger logger)
        {
            _logger = logger;
        }

        public CMSResult DynamicsSendMessage(SmsModel model)
        {
            var cmsResult = new CMSResult();
            String WebResponseString = "";
            String URL = "";
            
            try
            {
                //  URL = string.Format("http://103.16.101.52:8080/bulksms/bulksms?username=" + model.UserName + "&password=" + model.Password + "&type=0&dlr=1&destination=" + model.SendTo + "&source=" + model.SenderId + "&message=" + model.Message + "");
                URL = "http://173.45.76.227/send.aspx?username=" + model.UserName + "&pass=" + model.Password + "&route=trans1&senderid=" + model.SenderId + "&numbers=" + model.SendTo + "&message=" + model.Message + "";
                WebRequest webrequest;
                WebResponse webresponse;
                webrequest = HttpWebRequest.Create(URL);//Hit URL Link
                webrequest.Timeout = 25000;
                try
                {
                    webresponse = webrequest.GetResponse();//Get Response
                    StreamReader reader = new StreamReader(webresponse.GetResponseStream()); //Read Response and store in variable
                    WebResponseString = reader.ReadToEnd();
                    webresponse.Close();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + "SendMessage");
                    WebResponseString = "Request Timeout";
                    cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                    return cmsResult;
                }

                #region ReturnCodeDetails
                //1701: Success, Message Submitted Successfully, In this case you will  receive
                //the response 1701|<CELL_NO>:<MESSAGE ID>, The message Id can
                //then be used later to map the delivery reports to this message.
                //1702: Invalid URL Error, This means that one of the parameters was not
                //provided or left blank
                //1703: Invalid value in username or password field
                //1704: Invalid value in "type" field
                //1705: Invalid Message
                //1706: Invalid Destination
                //1707: Invalid Source (Sender)
                //1708: Invalid value for "dlr" field
                //1709: User validation failed
                //1710: Internal Error
                //1025 :Insufficient Credit
                //1032 : Destination in DND
                //1033 : Sender / Template Mismatch 
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + "SendMessage 1");
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                return cmsResult;
            }

            var getCode = WebResponseString.Split('|')[0];
            int sentCount = getCode == "1" ? Convert.ToInt32(Convert.ToInt32(WebResponseString.Split('|')[1]) / model.SendTo.Split(',').Count()) + 1 : 0;
            int failCount = model.SendTo.Split(',').Count() - sentCount;
            //failCount += WebResponseString.Split('|').Count(f => f.Contains("1705"));
            //failCount += WebResponseString.Split('|').Count(f => f.Contains("1706"));
            if (getCode == "1")
            {
                cmsResult.Results.Add(new Result { Message = sentCount + " SMS sent successfully and " + failCount + " SMS sent failed.", IsSuccessful = true });
                #region CreateSMSFileLog
                //List<string> mobileNoList = new List<string>();
                //foreach (var resultCode in WebResponseString.Split(','))
                //{
                //    if (resultCode.Split('|')[0] == "1025")
                //    {
                //        mobileNoList.Add(resultCode.Split('|')[1]);
                //    }
                //}
                //if (mobileNoList.Count > 0)
                //{
                //    var list = string.Join(",", mobileNoList);
                //    var filePath = string.Format(@"~/Images/{0}/smsLogFile.txt", Constants.SMSFileFolder);
                //    using (StreamWriter writetext = new StreamWriter(filePath, true))
                //    {
                //        writetext.WriteLine(model.Message + "," + list);
                //    }
                //    cmsResult.Results.Add(new Result { Message = "Some message sent successfully. Because of insufficient credit.", IsSuccessful = false });
                //    return cmsResult;
                //}                 
                #endregion
            }
            else if (getCode == "2")
            {
                cmsResult.Results.Add(new Result { Message = "Invalid Credentials.", IsSuccessful = false });
            }
            else if (getCode == "3")
            {
                cmsResult.Results.Add(new Result { Message = "Insufficient credit for sending SMS.", IsSuccessful = false });
            }
            else if (getCode == "4")
            {
                cmsResult.Results.Add(new Result { Message = "Error.", IsSuccessful = false });
            }
            else if (getCode == "5")
            {
                cmsResult.Results.Add(new Result { Message = "Invalid senderid.", IsSuccessful = false });
            }
            else if (getCode == "6")
            {
                cmsResult.Results.Add(new Result { Message = "Invalid route.", IsSuccessful = false });
            }
            else if (getCode == "7")
            {
                cmsResult.Results.Add(new Result { Message = "Submission Error.", IsSuccessful = false });
            }

            return cmsResult;
        }
        public CMSResult SendMessage(SmsModel model)
        {
            var cmsResult = new CMSResult();
            String WebResponseString = "";
            String URL = "";
            
            model.UserName = ConfigurationManager.AppSettings[Constants.UserName];
            model.Password = ConfigurationManager.AppSettings[Constants.SmsPassword];
            model.SenderId = ConfigurationManager.AppSettings[Constants.SenderId];
            try
            {
              //  URL = string.Format("http://103.16.101.52:8080/bulksms/bulksms?username=" + model.UserName + "&password=" + model.Password + "&type=0&dlr=1&destination=" + model.SendTo + "&source=" + model.SenderId + "&message=" + model.Message + "");
             //  URL = "http://173.45.76.227/send.aspx?username=" + model.UserName + "&pass=" + model.Password + "&route=trans1&senderid=" + model.SenderId + "&numbers=" + model.SendTo + "&message=" + model.Message + "";
                WebRequest webrequest;
                WebResponse webresponse;
                webrequest = HttpWebRequest.Create(URL);//Hit URL Link
                webrequest.Timeout = 25000;
                try
                {
                    webresponse = webrequest.GetResponse();//Get Response
                    StreamReader reader = new StreamReader(webresponse.GetResponseStream()); //Read Response and store in variable
                    WebResponseString = reader.ReadToEnd();
                    webresponse.Close();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + "SendMessage");
                    WebResponseString = "Request Timeout";
                    cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                    return cmsResult;
                }

                #region ReturnCodeDetails
                //1701: Success, Message Submitted Successfully, In this case you will  receive
                //the response 1701|<CELL_NO>:<MESSAGE ID>, The message Id can
                //then be used later to map the delivery reports to this message.
                //1702: Invalid URL Error, This means that one of the parameters was not
                //provided or left blank
                //1703: Invalid value in username or password field
                //1704: Invalid value in "type" field
                //1705: Invalid Message
                //1706: Invalid Destination
                //1707: Invalid Source (Sender)
                //1708: Invalid value for "dlr" field
                //1709: User validation failed
                //1710: Internal Error
                //1025 :Insufficient Credit
                //1032 : Destination in DND
                //1033 : Sender / Template Mismatch 
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + "SendMessage 1");
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
                return cmsResult;
            }

            var getCode = WebResponseString.Split('|')[0];
            int sentCount = getCode == "1" ? Convert.ToInt32(Convert.ToInt32(WebResponseString.Split('|')[1]) / model.SendTo.Split(',').Count()) + 1 : 0;
            int failCount = model.SendTo.Split(',').Count() - sentCount;
            //failCount += WebResponseString.Split('|').Count(f => f.Contains("1705"));
            //failCount += WebResponseString.Split('|').Count(f => f.Contains("1706"));
            if (getCode == "1")
            {
                cmsResult.Results.Add(new Result { Message = sentCount + " SMS sent successfully and " + failCount + " SMS sent failed.", IsSuccessful = true });
                #region CreateSMSFileLog
                //List<string> mobileNoList = new List<string>();
                //foreach (var resultCode in WebResponseString.Split(','))
                //{
                //    if (resultCode.Split('|')[0] == "1025")
                //    {
                //        mobileNoList.Add(resultCode.Split('|')[1]);
                //    }
                //}
                //if (mobileNoList.Count > 0)
                //{
                //    var list = string.Join(",", mobileNoList);
                //    var filePath = string.Format(@"~/Images/{0}/smsLogFile.txt", Constants.SMSFileFolder);
                //    using (StreamWriter writetext = new StreamWriter(filePath, true))
                //    {
                //        writetext.WriteLine(model.Message + "," + list);
                //    }
                //    cmsResult.Results.Add(new Result { Message = "Some message sent successfully. Because of insufficient credit.", IsSuccessful = false });
                //    return cmsResult;
                //}                 
                #endregion
            }
            else if (getCode == "2")
            {
                cmsResult.Results.Add(new Result { Message = "Invalid Credentials.", IsSuccessful = false });
            }
            else if (getCode == "3")
            {
                cmsResult.Results.Add(new Result { Message = "Insufficient credit for sending SMS.", IsSuccessful = false });
            }
            else if (getCode == "4")
            {
                cmsResult.Results.Add(new Result { Message = "Error.", IsSuccessful = false });
            }
            else if (getCode == "5")
            {
                cmsResult.Results.Add(new Result { Message = "Invalid senderid.", IsSuccessful = false });
            }
            else if (getCode == "6")
            {
                cmsResult.Results.Add(new Result { Message = "Invalid route.", IsSuccessful = false });
            }
            else if (getCode == "7")
            {
                cmsResult.Results.Add(new Result { Message = "Submission Error.", IsSuccessful = false });
            }

            return cmsResult;
        }

        public string CreditCheck()
        {

            String WebResponseString = "";
            try
            {
                var userName = ConfigurationManager.AppSettings[Constants.UserName];
                var password = ConfigurationManager.AppSettings[Constants.SmsPassword];
               // String URL = "http://173.45.76.227/balance.aspx?username=" + userName + "&pass=" + password + "";
                String URL = "http://103.16.101.52:8080/CreditCheck/checkcredits?username=" + userName + "&password=" + password + "";
                WebRequest webrequest; WebResponse webresponse;
                webrequest = HttpWebRequest.Create(URL);//Hit URL Link
                webrequest.Timeout = 5000;

                try
                {
                    webresponse = webrequest.GetResponse();//Get Response
                    StreamReader reader = new StreamReader(webresponse.GetResponseStream()); //Read Response and store in variable
                    WebResponseString = reader.ReadToEnd();
                    webresponse.Close();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + "CreditCheck");
                    WebResponseString = "Request Timeout";
                    return ex.Message;
                    #region result code
                    //1701: Success, Message Submitted Successfully, In this case you will  receive
                    //the response 1701|<CELL_NO>:<MESSAGE ID>, The message Id can
                    //then be used later to map the delivery reports to this message.
                    //1702: Invalid URL Error, This means that one of the parameters was not
                    //provided or left blank
                    //1703: Invalid value in username or password field
                    //1704: Invalid value in "type" field
                    //1705: Invalid Message
                    //1706: Invalid Destination
                    //1707: Invalid Source (Sender)
                    //1708: Invalid value for "dlr" field
                    //1709: User validation failed
                    //1710: Internal Error
                    //1025 :Insufficient Credit
                    //1032 : Destination in DND
                    //1033 : Sender / Template Mismatch 
                    #endregion
                }
            }
            catch
            {

            }
            return WebResponseString;
        }

        public void StartProcessing(SmsModel[] smsModels, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                {
                    foreach (var smsModel in smsModels)
                    {
                        //execute when task has been cancel  
                        cancellationToken.ThrowIfCancellationRequested();
                        //send sms here
                        //  SendMessage(smsModel); // Default SMS Sending
                        DynamicsSendMessage(smsModel); //Dynamic SMS Sending
                        Thread.Sleep(1500);   // wait to 1.5 sec every time  
                    }
                    _logger.Info("SMS Send Successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }
    }
}