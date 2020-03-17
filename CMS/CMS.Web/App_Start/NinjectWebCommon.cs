[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CMS.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CMS.Web.App_Start.NinjectWebCommon), "Stop")]

namespace CMS.Web.App_Start
{
    using Domain.Infrastructure;
    using Domain.Storage;
    using Domain.Storage.Services;
    using Helpers;
    using Infrastructure;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            log4net.Config.XmlConfigurator.Configure();
            var kernel = new StandardKernel(new LoggingModule());
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                kernel.Bind<IRepository>().To<Repository>().InRequestScope().WithConstructorArgument("context", new CMSDbContext());
                kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
                kernel.Bind<IClassService>().To<ClassService>();
                kernel.Bind<IBatchService>().To<BatchService>();
                kernel.Bind<IChapterService>().To<ChapterService>();
                kernel.Bind<ISubjectService>().To<SubjectService>();
                kernel.Bind<IQuestionService>().To<QuestionService>();
                kernel.Bind<IBoardService>().To<BoardService>();
                kernel.Bind<IInstallmentService>().To<InstallmentService>();
                kernel.Bind<IMasterFeeService>().To<MasterFeeService>();
                kernel.Bind<IStudentService>().To<StudentService>();
                kernel.Bind<IApplicationUserService>().To<ApplicationUserService>();
                kernel.Bind<IEmailService>().To<EmailService>();
                kernel.Bind<IPDFUploadService>().To<PDFUploadService>();
                kernel.Bind<IPDFCategoryService>().To<PDFCategoryService>();
                kernel.Bind<IUploadNotesService>().To<UploadNotesService>();
                kernel.Bind<IUploadReferencebooksService>().To<UploadReferencebooksService>();
                kernel.Bind<IUploadAssignmentsService>().To<UploadAssignmentsService>();
                kernel.Bind<IUploadTextbooksService>().To<UploadTextbooksService>();
                kernel.Bind<IUploadInbuiltquestionbankService>().To<UploadInbuiltquestionbankService>();
                kernel.Bind<IUploadPracticepapersService>().To<UploadPracticepapersService>();
                kernel.Bind<IUploadQuestionpapersService>().To<UploadQuestionpapersService>();
                kernel.Bind<ITeacherService>().To<TeacherService>();
                kernel.Bind<IAttendanceService>().To<AttendanceService>();
                kernel.Bind<ITestPaperService>().To<TestPaperService>();
                kernel.Bind<IMachineService>().To<MachineService>();
                kernel.Bind<IBranchService>().To<BranchService>();
                kernel.Bind<IClientService>().To<ClientService>();
                kernel.Bind<ISchoolService>().To<SchoolService>();
                kernel.Bind<IBranchAdminService>().To<BranchAdminService>();
                kernel.Bind<IClientAdminService>().To<ClientAdminService>();
                kernel.Bind<IAspNetRoles>().To<AspNetRoles>();
                kernel.Bind<IAnnouncementService>().To<AnnouncementService>();
                kernel.Bind<ISmsService>().To<SmsService>();
                kernel.Bind<ISendNotificationService>().To<SendNotificationService>();
                kernel.Bind<INotificationService>().To<NotificationService>();
                kernel.Bind<IApiService>().To<ApiService>();
                kernel.Bind<IStudentFeedbackService>().To<StudentFeedbackService>();
                kernel.Bind<IStudentTimetableService>().To<StudentTimetableService>();
                kernel.Bind<IDailyPracticePaperService>().To<DailyPracticePaperService>();
                kernel.Bind<IOfflineTestPaper>().To<OfflineTestPaperService>();
                kernel.Bind<ILocalDateTimeService>().To<LocalDateTimeService>();
                kernel.Bind<IOfflineTestStudentMarksService>().To<OfflineTestStudentMarksService>();
                kernel.Bind<IArrangeTestResultService>().To<ArrangeTestResultService>();
                kernel.Bind<IConfigureServices>().To<ConfigureService>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
        }
    }
}
