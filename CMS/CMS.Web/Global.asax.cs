using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Web.App_Start;
using CMS.Web.Logger;
using CMS.Web.Scheduler;
using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CMS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperWebConfig.Configure();
            JobScheduler.Start();
        
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true; // **** Always accept
    };
            string UploadResultPDF = Server.MapPath(string.Concat("~/PDF/", Constants.UploadResultPDF));
            string UploadTestsPDF = Server.MapPath(string.Concat("~/PDF/", Constants.UploadTestsPDF));
            string UploadTestsLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadTestsLogo));
            string UploadNewsPDF = Server.MapPath(string.Concat("~/PDF/", Constants.UploadNewsPDF));
            string WriteBlogPDF = Server.MapPath(string.Concat("~/PDF/", Constants.WriteBlog));
            string BlogImage = Server.MapPath(string.Concat("~/Images/", Constants.BlogImage));
            string StudentImagePath = Server.MapPath(string.Concat("~/Images/", Constants.StudentImageFolder));
            string StudentTimeTableFile = Server.MapPath(string.Concat("~/PDF/", Constants.StudentTimeTableFile));
            string DailyPracticePaperFile = Server.MapPath(string.Concat("~/PDF/", Constants.DailyPracticePaperFile));

            

            if (!Directory.Exists(UploadResultPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadResultPDF);
            }

            string UploadNotesPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadNotesPDF));

            if (!Directory.Exists(UploadNotesPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadNotesPDF);
            }

            string UploadNotesLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadNotesLogo));

            if (!Directory.Exists(UploadNotesLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadNotesLogo);
            }

            string UploadAssignmentsPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadAssignmentsPDF));

            if (!Directory.Exists(UploadAssignmentsPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadAssignmentsPDF);
            }

            string UploadAssignmentsLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadAssignmentsLogo));

            if (!Directory.Exists(UploadAssignmentsLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadAssignmentsLogo);
            }

            string UploadTextbooksPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadTextbooksPDF));

            if (!Directory.Exists(UploadTextbooksPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadTextbooksPDF);
            }

            string UploadTextbooksLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadTextbooksLogo));

            if (!Directory.Exists(UploadTextbooksLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadTextbooksLogo);
            }

            string UploadInbuiltquestionbankPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadInbuiltquestionbankPDF));

            if (!Directory.Exists(UploadInbuiltquestionbankPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadInbuiltquestionbankPDF);
            }

            string UploadInbuiltquestionbankLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadInbuiltquestionbankLogo));

            if (!Directory.Exists(UploadInbuiltquestionbankLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadInbuiltquestionbankLogo);
            }

            string UploadPracticepapersPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadPracticepapersPDF));

            if (!Directory.Exists(UploadPracticepapersPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadPracticepapersPDF);
            }

            string UploadPracticepapersLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadPracticepapersLogo));

            if (!Directory.Exists(UploadPracticepapersLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadPracticepapersLogo);
            }

            string UploadQuestionpapersPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadQuestionpapersPDF));

            if (!Directory.Exists(UploadQuestionpapersPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadQuestionpapersPDF);
            }

            string UploadQuestionpapersLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadQuestionpapersLogo));

            if (!Directory.Exists(UploadQuestionpapersLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadQuestionpapersLogo);
            }


            string UploadReferencebooksPDF = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadReferencebooksPDF));

            if (!Directory.Exists(UploadReferencebooksPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadReferencebooksPDF);
            }

            string UploadReferencebooksLogo = Server.MapPath(string.Concat("~/Images/", Constants.UploadReferencebooksLogo));

            if (!Directory.Exists(UploadReferencebooksLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadReferencebooksLogo);
            }



            if (!Directory.Exists(UploadTestsPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadTestsPDF);
            }
            if (!Directory.Exists(UploadTestsLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadTestsLogo);
            }
            /*if (!Directory.Exists(UploadAssignmentsPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadAssignmentsPDF);
            }
            if (!Directory.Exists(UploadAssignmentsLogo))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadAssignmentsLogo);
            }*/
            if (!Directory.Exists(UploadNewsPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadNewsPDF);
            }
            if (!Directory.Exists(WriteBlogPDF))
            {
                DirectoryInfo di = Directory.CreateDirectory(WriteBlogPDF);
            }
            string StudentImageFolder = Server.MapPath(string.Concat("~/Images/", Constants.StudentImageFolder));

            if (!Directory.Exists(StudentImageFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(StudentImageFolder);
            }

            string UploadPhotos = Server.MapPath(string.Concat("~/Images/", Constants.UploadPhotos));

            if (!Directory.Exists(UploadPhotos))
            {
                DirectoryInfo di = Directory.CreateDirectory(UploadPhotos);
            }
            if (!Directory.Exists(BlogImage))
            {
                DirectoryInfo di = Directory.CreateDirectory(BlogImage);
            }

            if (!Directory.Exists(StudentImagePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(StudentImagePath);
            }

            string QuestionImagePath = Server.MapPath(string.Concat("~/Images/", Constants.QuestionImageFolder));

            if (!Directory.Exists(QuestionImagePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(QuestionImagePath);
            }

            string PdfFilePath = Server.MapPath(string.Concat("~/PDF/", Constants.PdfFileFolder));

            if (!Directory.Exists(PdfFilePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(PdfFilePath);
            }

            string BrochureFilePath = Server.MapPath(string.Concat("~/PDF/", Constants.BrochureFolder));

            if (!Directory.Exists(BrochureFilePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(BrochureFilePath);
            }

            string SMSFileFolder = Server.MapPath(string.Concat("~/Images/", Constants.SMSFileFolder));

            if (!Directory.Exists(SMSFileFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(SMSFileFolder);
            }
            if (!Directory.Exists(StudentTimeTableFile))
            {
                DirectoryInfo di = Directory.CreateDirectory(StudentTimeTableFile);
            }
            if (!Directory.Exists(DailyPracticePaperFile))
            {
                DirectoryInfo di = Directory.CreateDirectory(DailyPracticePaperFile);
            }
        }

        protected void Application_EndRequest()
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();
            var repository = DependencyResolver.Current.GetService<IRepository>();
            if (HttpContext.Current.Error == null && repository != null)
            {
                repository.CommitChanges();
            }
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;
        }
    }
}
