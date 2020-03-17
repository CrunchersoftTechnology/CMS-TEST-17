using CMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CMS.Domain.Storage
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<CMSDbContext>
    {
        protected override void Seed(CMSDbContext context)
        {
            //IList<Class> defaultClasses = new List<Class>();
            //defaultClasses.Add(new Class() { ClassId = 1, Name = "10A", CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Class cls in defaultClasses)
            //    context.Set<Class>().Add(cls);



            //IList<Subject> defaultSubjects = new List<Subject>();
            //defaultSubjects.Add(new Subject() { SubjectId = 1, Name = "Mathematics", ClassId = 1, CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Subject sub in defaultSubjects)
            //    context.Set<Subject>().Add(sub);


            #region Batches
            //var bList = new List<Batch>
            //{
            //    new Batch()
            //    {
            //        BatchId = 1,
            //        SubjectId = 1,
            //        Name = "Morning Batch",
            //        InTime = Convert.ToDateTime(DateTime.UtcNow),
            //        OutTime = Convert.ToDateTime(DateTime.UtcNow),
            //        CreatedOn = DateTime.UtcNow,
            //        CreatedBy = "admin@gmail.com"
            //    },
            //    new Batch()
            //    {
            //        BatchId = 2,
            //        SubjectId = 2,
            //        Name = "Morning Batch",
            //        InTime = Convert.ToDateTime(DateTime.UtcNow),
            //        OutTime = Convert.ToDateTime(DateTime.UtcNow),
            //        CreatedOn = DateTime.UtcNow,
            //        CreatedBy = "admin@gmail.com"
            //    }
            //};

            //foreach (Batch b in bList)
            //    context.Set<Batch>().Add(b);
            #endregion

            //IList<Chapter> defaultChapters = new List<Chapter>();
            //defaultChapters.Add(new Chapter() { ChapterId = 1, Name = "Time and Work",SubjectId=1,Weightage=8, CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Chapter ch in defaultChapters)
            //    context.Set<Chapter>().Add(ch);

            //IList<MasterFee> defaultMFees = new List<MasterFee>();
            //defaultMFees.Add(new MasterFee() { MasterFeeId = 1, Year = "2016-2017",SubjectId=1,ClassId=1,Fee=Convert.ToDecimal(6000.00), CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (MasterFee mfee in defaultMFees)
            //    context.Set<MasterFee>().Add(mfee);

            //IList<Board> defaultBoards = new List<Board>();
            //defaultBoards.Add(new Board() { BoardId = 1, Name = "HSC Board", CreatedOn = DateTime.UtcNow, CreatedBy = "admin@gmail.com" });
            //foreach (Board ba in defaultBoards)
            //    context.Set<Board>().Add(ba);

            base.Seed(context);
        }
    }
}
