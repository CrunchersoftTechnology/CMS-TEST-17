namespace CMS.Domain.Storage.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CMS.Domain.Storage.CMSDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CMS.Domain.Storage.CMSDbContext context)
        {
            //IList<Class> defaultClasses = new List<Class>();
            //defaultClasses.Add(new Class() { ClassId = 1, Name = "10A", CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Class cls in defaultClasses)
            //    context.Set<Class>().Add(cls);

            //IList<Batch> defaultBatches = new List<Batch>();
            //defaultBatches.Add(new Batch() { BatchId = 1, ClassId = 1, Name = "Morning Batch", InTime = Convert.ToDateTime(DateTime.UtcNow), OutTime = Convert.ToDateTime(DateTime.UtcNow), CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Batch b in defaultBatches)
            //    context.Set<Batch>().Add(b);

            //IList<Subject> defaultSubjects = new List<Subject>();
            //defaultSubjects.Add(new Subject() { SubjectId = 1, Name = "Mathematics", ClassId = 1, CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Subject sub in defaultSubjects)
            //    context.Set<Subject>().Add(sub);

            //IList<Chapter> defaultChapters = new List<Chapter>();
            //defaultChapters.Add(new Chapter() { ChapterId = 1, Name = "Time and Work", SubjectId = 1, Weightage = 8, CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Chapter ch in defaultChapters)
            //    context.Set<Chapter>().Add(ch);

            //IList<MasterFee> defaultMFees = new List<MasterFee>();
            //defaultMFees.Add(new MasterFee() { MasterFeeId = 1, Year = "2016-2017", SubjectId = 1, ClassId = 1, Fee = Convert.ToDecimal(6000.00), CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (MasterFee mfee in defaultMFees)
            //    context.Set<MasterFee>().Add(mfee);

            //IList<Board> defaultBoards = new List<Board>();
            //defaultBoards.Add(new Board() { BoardId = 1, Name = "HSC Board", CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Board ba in defaultBoards)
            //    context.Set<Board>().Add(ba);

            //IList<Installment> defaultInstallments = new List<Installment>();
            //defaultInstallments.Add(new Installment() { InstallmentId = 1, Payment = Convert.ToDecimal(5000.00), CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Installment ins in defaultInstallments)
            //    context.Set<Installment>().Add(ins);

            base.Seed(context);
        }
    }
}
