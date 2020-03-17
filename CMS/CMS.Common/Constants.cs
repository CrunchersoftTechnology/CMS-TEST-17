namespace CMS.Common
{
    public static class Constants
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";
        public const string StudentRole = "Student";
        public const string TeacherRole = "Teacher";
        public const string ClientAdminRole = "Client";
        public const string TimeFormat = "hh:mm tt";
        public const string DateFormat = "dd/MM/YYYY";
        public const string StudentImageFolder = "StudentImage";
        public const string QuestionImageFolder = "QuestionImage";
        public const string PdfFileFolder = "PdfFile";
        public static readonly string[] ImageTypes = { "image/jpg", "image/jpeg", "image/png" };
        public const string PdfType = "application/pdf";
        public const string SMSFileFolder = "SMSLogFile";

        #region Email/SMTP Settings

        public const string FromEmail = "FromEmail";
        public const string SmtpServer = "SmtpServer";
        public const string SmtpPort = "SmtpPort";
        public const string EmailPassword = "EmailPassword";

        #endregion

        #region SMS Settings

        public const string UserName = "SmsUserName";
        public const string SmsPassword = "SmsPassword";
        public const string SenderId = "SmsSenderId";

        #endregion

        #region Send Notification

        public const string ParentAppId = "ParentAppId";
        public const string ParentRestAppId = "ParentRestAppId";
        public const string StudentAppId = "StudentAppId";
        public const string StudentRestAppId = "StudentRestAppId";

        #endregion

        public const string BrochureFolder = "BrochureFile";
        public const string BranchAdminRole = "BranchAdmin";
        public const string UploadResultPDF = "UploadResultPDF";

        public const string UploadNotesPDF = "UploadNotesPDF";
        public const string UploadNotesLogo = "UploadNotesLogo";

        public const string UploadReferencebooksPDF = "UploadReferencebooksPDF";
        public const string UploadReferencebooksLogo = "UploadReferencebooksLogo";

        public const string UploadTextbooksPDF = "UploadTextbooksPDF";
        public const string UploadTextbooksLogo = "UploadTextbooksLogo";

        public const string UploadInbuiltquestionbankPDF = "UploadInbuiltquestionbankPDF";
        public const string UploadInbuiltquestionbankLogo = "UploadInbuiltquestionbankLogo";

        public const string UploadPracticepapersPDF = "UploadPracticepapersPDF";
        public const string UploadPracticepapersLogo = "UploadPracticepapersLogo";

        public const string UploadQuestionpapersPDF = "UploadQuestionpapersPDF";
        public const string UploadQuestionpapersLogo = "UploadQuestionpapersLogo";

        public const string UploadTestsPDF = "UploadTestsPDF";
        public const string UploadTestsLogo = "UploadTestsLogo";
        public const string UploadAssignmentsPDF = "UploadAssignmentsPDF";
        public const string UploadAssignmentsLogo = "UploadAssignmentsLogo";
        public const string UploadNewsPDF = "UploadNewsPDF";
        public const string UploadPhotos = "UploadPhotos";
        public const string WriteBlog = "WriteBlog";
        public const string UrlClasses = "UrlClasses";
        public const string UrlBoards = "UrlBoards";
        public const string UrlSubject = "UrlSubject";
        public const string UrlTeacher = "UrlTeacher";
        public const string UrlStudent = "UrlStudent";
        public const string UrlSchool = "UrlSchool";
        public const string UrlBranch = "UrlBranch";
        public const string UrlClient = "UrlClient";
        public const string AdminEmail = "AdminEmail";
        public const string UrlBatch = "UrlBatch";
        public const string UrlAdmission = "UrlAdmission";
        public const string UrlBrochure = "UrlBrochure";
        public const string UrlPickAndDrop = "UrlPickAndDrop";
        public const string UrlStudentFeedback = "UrlStudentFeedback";
        public const string UrlStudentFeedbackPost = "UrlStudentFeedbackPost";
        public const string UrlClassesByMultipleBranch = "UrlClassesByMultipleBranch";
        public const string BlogImage = "BlogImage";
        public const string UrlBlog = "UrlBlog";
        public const string StudentAppPDF = "StudentAppPDF";
        public const string OnlineTest = "onlineTest";
        public const string UrlNews = "UrlNews";
        public const string urlNewsPDF = "urlNewsPDF";

        public const string UrlNotes = "UrlNotes";
        public const string UrlAssignments = "UrlAssignments";
        public const string UrlTextbooks = "UrlTextbooks";
        public const string UrlInbuiltquestionbank = "UrlInbuiltquestionbank";
        public const string UrlPracticepapers = "UrlPracticepapers";
        public const string UrlQuestionpapers = "UrlQuestionpapers";
        public const string UrlReferencebooks = "UrlReferencebooks";

        public const string UrlTests = "UrlTests";
        public const string UrlWebsiteImage = "UrlWebsiteImage";
        public const string UrlNewsPDF = "UrlNewsPDF";
        public const string UrlTestPDF = "UrlTestPDF";

        public const string UrlAssignmentPDF = "UrlAssignmentPDF";
        public const string UrlNotesPDF = "UrlNotesPDF";
        public const string UrlReferencebooksPDF = "UrlReferencebooksPDF";
        public const string UrlTextbooksPDF = "UrlTextbooksPDF";
        public const string UrlInbuiltquestionbankPDF = "UrlInbuiltquestionbankPDF";
        public const string UrlPracticepapersPDF = "UrlPracticepapersPDF";
        public const string UrlQuestionpapersPDF = "UrlQuestionpapersPDF";

        public const string StudentTimeTableFile = "StudentTimeTableFile";
        public const string DailyPracticePaperFile = "DailyPracticePaperFile";
        public const string UrlTestsById = "UrlTestsById";

        public const string UrlNotesById = "UrlNotesById";
        public const string UrlReferencebooksById = "UrlReferencebooksById";
        public const string UrlAssignmentById = "UrlAssignmentById";
        public const string UrlTextbooksById = "UrlTextbooksById";
        public const string UrlInbuiltquestionbankById = "UrlInbuiltquestionbankById";
        public const string UrlPracticepapersById = "UrlPracticepapersById";
        public const string UrlQuestionpapersById = "UrlQuestionpapersById";


        public const string UrlAdmissionUpdate = "UrlAdmissionUpdate";
        public const string UrlGetPendingAdmission = "UrlGetPendingAdmission";
        public const string UrlGetPendingAdmissionList = "UrlGetPendingAdmissionList";
        public const string IntervalInHoursValue = "IntervalInHoursValue";
        public const string PDFCategoryApi = "UrlGetPdf";
      
        #region App links
        public const string parentAppLink = "parentAppLink";
        public const string studentAppLink = "studentAppLink";
        #endregion




    }
}
