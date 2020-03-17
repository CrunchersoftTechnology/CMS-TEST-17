using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Mappings;
using CMS.Domain.Storage.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace CMS.Domain.Storage
{
    public class CMSDbContext : IdentityDbContext<ApplicationUser>
    {
        public CMSDbContext() : base("Name=CMSWebConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new DbInitializer());
        }

      public IDbSet<Batch> Batches { get; set; }
        IDbSet<Configure> Configures { get; set; }
        IDbSet<Chapter> Chapters { get; set; }
        IDbSet<Class> Classes { get; set; }
        IDbSet<Question> Questions { get; set; }
        IDbSet<Subject> Subjects { get; set; }
        IDbSet<Board> Boards { get; set; }
        IDbSet<Installment> Installments { get; set; }
        IDbSet<MasterFee> MasterFees { get; set; }
        IDbSet<Student> Students { get; set; }
        IDbSet<PDFUpload> PDFUploads { get; set; }
        IDbSet<PDFCategory> PDFCategories { get; set; }
        IDbSet<Teacher> Teachers { get; set; }
        IDbSet<Attendance> Attendances { get; set; }
        IDbSet<TestPaper> TestPapers { get; set; }
        IDbSet<Machine> Machines { get; set; }
        IDbSet<Branch> Branches { get; set; }
        IDbSet<Client> Clients { get; set; }
        IDbSet<School> Schools { get; set; }
        IDbSet<BranchAdmin> BranchAdmins { get; set; }
        IDbSet<ClientAdmin> ClientAdmins { get; set; }
        IDbSet<Announcement> Announcements { get; set; }
        IDbSet<Notification> Notifications { get; set; }
        IDbSet<ArrengeTest> ArrengeTests { get; set; }
        IDbSet<StudentFeedback> StudentFeedbacks { get; set; }
        IDbSet<StudentTimetable> StudentTimetables { get; set; }
        IDbSet<DailyPracticePaper> DailyPracticePapers { get; set; }
        IDbSet<OfflineTestPaper> OfflineTestPaper { get; set; }
        IDbSet<OfflineTestStudentMarks> OfflineTestStudentMarks { get; set; }
        IDbSet<ArrangeTestResult> ArrangeTestResults { get; set; }
        IDbSet<UploadNotes> UploadNotes { get; set; }
        IDbSet<UploadAssignments> UploadAssignments { get; set; }
        IDbSet<UploadReferencebooks> UploadReferencebooks { get; set; }
        IDbSet<UploadTextbooks> UploadTextbooks { get; set; }
        IDbSet<UploadInbuiltquestionbank> UploadInbuiltquestionbank { get; set; }
        IDbSet<UploadPracticepapers> UploadPracticepapers { get; set; }
        IDbSet<UploadQuestionpapers> UploadQuestionpapers { get; set; }

       
     

        public static CMSDbContext Create()
        {
            return new CMSDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ClassMap());
            modelBuilder.Configurations.Add(new StudentMap());
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Student>()
                .HasMany<Subject>(s => s.Subjects)
                .WithMany(c => c.Students)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("SubjectId");
                    cs.ToTable("StudentSubjects");
                });

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            try
            {
                var modifiedEntries = ChangeTracker.Entries()
                       .Where(x => x.Entity is AuditableEntity
                           && (x.State == System.Data.Entity.EntityState.Added || x.State == System.Data.Entity.EntityState.Modified));

                foreach (var entry in modifiedEntries)
                {
                    AuditableEntity entity = entry.Entity as AuditableEntity;
                    if (entity != null)
                    {
                        string identityName = Thread.CurrentPrincipal.Identity.Name;
                        LocalDateTimeService obj = new LocalDateTimeService();
                        DateTime now = obj.GetDateTime();

                        if (entry.State == System.Data.Entity.EntityState.Added)
                        {
                            entity.CreatedBy = identityName;
                            entity.CreatedOn = now;
                        }
                        else if (entry.State == System.Data.Entity.EntityState.Modified)
                        {
                            entity.UpdatedBy = identityName;
                            entity.UpdatedOn = now;
                        }
                        else
                        {
                            base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            base.Entry(entity).Property(x => x.CreatedOn).IsModified = false;
                            entity.UpdatedBy = identityName;
                            entity.UpdatedOn = now;
                        }
                    }
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public System.Data.Entity.DbSet<CMS.Domain.Models.UserAccount> UserAccounts { get; set; }

        //     public System.Data.Entity.DbSet<CMS.Domain.Models.Configure> Configures { get; set; }


    }
}
