using CMS.Common;
using System.Configuration;
using System.Net;

namespace CMS.Domain.Storage.Services
{
    public class ApiService :IApiService 
    {

        public object JsonConvert { get; private set; }

        public string GetBoards()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                var urlBoards = ConfigurationManager.AppSettings[Constants.UrlBoards];
                var boardresult = client.DownloadString(urlBoards);
                return boardresult;
            }
        }

        public string GetClasses()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };//trupti
                var urlClasses = ConfigurationManager.AppSettings[Constants.UrlClasses];
                var classResult = client.DownloadString(urlClasses);
                return classResult;
            }
        }

        public string GetSubjects(int classId)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                var urlSubject = ConfigurationManager.AppSettings[Constants.UrlSubject] + classId;
                var subjectResult = client.DownloadString(urlSubject);
                return subjectResult;
            }
        }

        public string GetTeachers()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };//trupti
                var urlTeachers = ConfigurationManager.AppSettings[Constants.UrlTeacher];
                var teacherResult = client.DownloadString(urlTeachers);
                return teacherResult;
            }
        }

        public string GetSchool()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                var urlSchool = ConfigurationManager.AppSettings[Constants.UrlSchool];
                var schoolResult = client.DownloadString(urlSchool);
                return schoolResult;
            }
        }

        public string GetBranch()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };//trupti
                var urlBranch = ConfigurationManager.AppSettings[Constants.UrlBranch];
                var branchResult = client.DownloadString(urlBranch);
                return branchResult;
            }
        }

        public string GetClient()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };//trupti
                var urlClient = ConfigurationManager.AppSettings[Constants.UrlClient];
                var clientResult = client.DownloadString(urlClient);
                return clientResult;
            }
        }


        public string GetBatch()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                var urlBatch = ConfigurationManager.AppSettings[Constants.UrlBatch];
                var batchResult = client.DownloadString(urlBatch);
                return batchResult;
            }
        }
        public string GetStudentFeedback()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };//trupti
                var urlstudentFeedback = ConfigurationManager.AppSettings[Constants.UrlStudentFeedback];
                var studentFeedbackResult = client.DownloadString(urlstudentFeedback);
                return studentFeedbackResult;
            }
        }
        public string GetClassesByMultipleBranch(string selectedBranch)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };//trupti
                var urlClasses = ConfigurationManager.AppSettings[Constants.UrlClassesByMultipleBranch] + selectedBranch; ;
                var classResult = client.DownloadString(urlClasses);
                return classResult;
            }
        }
         public string GetNews()
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlNews= ConfigurationManager.AppSettings[Constants.UrlNews];
                 var newsResult = client.DownloadString(urlNews);
                 return newsResult;

             }
         }

        public string GetNotes()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlNotes = ConfigurationManager.AppSettings[Constants.UrlNotes];
                var NotesResult = client.DownloadString(urlNotes);
                return NotesResult;

            }
        }
        public string GetTextbooks()
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlTextbooks = ConfigurationManager.AppSettings[Constants.UrlTextbooks];
                 var TextbooksResult = client.DownloadString(urlTextbooks);
                return TextbooksResult;

             }
         }

        public string GetInbuiltquestionbank()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlInbuiltquestionbank = ConfigurationManager.AppSettings[Constants.UrlInbuiltquestionbank];
                var InbuiltquestionbankResult = client.DownloadString(urlInbuiltquestionbank);
                return InbuiltquestionbankResult;

            }
        }

        public string GetPracticepapers()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlPracticepapers = ConfigurationManager.AppSettings[Constants.UrlPracticepapers];
                var PracticepapersResult = client.DownloadString(urlPracticepapers);
                return PracticepapersResult;

            }
        }

        public string GetQuestionpapers()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlQuestionpapers = ConfigurationManager.AppSettings[Constants.UrlQuestionpapers];
                var QuestionpapersResult = client.DownloadString(urlQuestionpapers);
                return QuestionpapersResult;

            }
        }
        public string GetReferencebooks()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlReferencebooks = ConfigurationManager.AppSettings[Constants.UrlReferencebooks];
                var ReferencebooksResult = client.DownloadString(urlReferencebooks);
                return ReferencebooksResult;

            }
        }
        public string GetAssignments()
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlAssignment = ConfigurationManager.AppSettings[Constants.UrlAssignments];
                 var assignmentResult = client.DownloadString(urlAssignment);
                 return assignmentResult;

             }
         }
         public string GetTest()
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlTest = ConfigurationManager.AppSettings[Constants.UrlTests];
                 var testResult = client.DownloadString(urlTest);
                 return testResult;

             }
         }

         public string GetAdmission(int? Id)
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlAdmission = ConfigurationManager.AppSettings[Constants.UrlAdmission]+ "?id="+Id;
                 var admissionResult = client.DownloadString(urlAdmission);
                 return admissionResult;

             }
         }

         public string GetTestByUploadId(int? Id)
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urltest = ConfigurationManager.AppSettings[Constants.UrlTestsById] + "?id=" + Id;
                 var testResult = client.DownloadString(urltest);
                 return testResult;

             }
         }

         public string GetNotesByUploadId(int? Id)
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlnotes = ConfigurationManager.AppSettings[Constants.UrlNotesById] + "?id=" + Id;
                 var testResult = client.DownloadString(urlnotes);
                 return testResult;

             }
         }

        public string GetTextbooksByUploadId(int? Id)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlTextbooks = ConfigurationManager.AppSettings[Constants.UrlTextbooksById] + "?id=" + Id;
                var testResult = client.DownloadString(urlTextbooks);
                return testResult;

            }
        }

        public string GetReferencebooksByUploadId(int? Id)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlReferencebooks = ConfigurationManager.AppSettings[Constants.UrlReferencebooksById] + "?id=" + Id;
                var testResult = client.DownloadString(urlReferencebooks);
                return testResult;

            }
        }


        public string GetInbuiltquestionbankByUploadId(int? Id)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlInbuiltquestionbank = ConfigurationManager.AppSettings[Constants.UrlInbuiltquestionbankById] + "?id=" + Id;
                var testResult = client.DownloadString(urlInbuiltquestionbank);
                return testResult;

            }
        }

        public string GetPracticepapersByUploadId(int? Id)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlPracticepapers = ConfigurationManager.AppSettings[Constants.UrlPracticepapersById] + "?id=" + Id;
                var testResult = client.DownloadString(urlPracticepapers);
                return testResult;

            }
        }

        public string GetQuestionpapersByUploadId(int? Id)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept:application/json");
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var urlQuestionpapers = ConfigurationManager.AppSettings[Constants.UrlQuestionpapersById] + "?id=" + Id;
                var testResult = client.DownloadString(urlQuestionpapers);
                return testResult;

            }
        }

        public string GetAssignmentByUploadId(int? Id)
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlAssignment = ConfigurationManager.AppSettings[Constants.UrlAssignmentById] + "?id=" + Id;
                 var testResult = client.DownloadString(urlAssignment);
                 return testResult;

             }
         }

         public string UpdateAdmission(string email)
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlAdmission = ConfigurationManager.AppSettings[Constants.UrlAdmissionUpdate] + "?email=" + email;
                 var admissionResult = client.DownloadString(urlAdmission);
                 return admissionResult;

             }
         }

         public string GetPendingAdmission()
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlPendingAdmission = ConfigurationManager.AppSettings[Constants.UrlGetPendingAdmission];
                 var admissionResult = client.DownloadString(urlPendingAdmission);
                 return admissionResult;

             }
         }

         public string GetPendingAdmissionList()
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlPendingAdmissionList = ConfigurationManager.AppSettings[Constants.UrlGetPendingAdmissionList];
                 var admissionResult = client.DownloadString(urlPendingAdmissionList);
                 return admissionResult;

             }
         }

        /* public string GetPdf(string category)
         {
             using (var client = new WebClient())
             {
                 client.Headers.Add("Accept:application/json");
                 ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                 var urlPendingAdmissionList = ConfigurationManager.AppSettings[Constants.UrlGetPdf];
                 var admissionResult = client.DownloadString(urlPendingAdmissionList);
                 return admissionResult;

             }
         }*/

        //public FileResult GetNewsPDFById(int id)
        //{
        //    using (var client = new WebClient())
        //    {
        //        client.Headers.Add("Content-Type:application/json");
        //        client.Headers.Add("Accept:application/json");
        //        ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        //        var result = client.UploadString(ConfigurationManager.AppSettings[Constants.urlNewsPDF], "POST", id.ToString());
        //        var response = client.ResponseHeaders;
        //    }
        //}
    }
}
