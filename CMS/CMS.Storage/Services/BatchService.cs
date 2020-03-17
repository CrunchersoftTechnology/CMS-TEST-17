using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;
using System;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class BatchService : IBatchService
    {
        readonly IRepository _repository;

        public BatchService(IRepository repository)
        {
            _repository = repository;
        }


        public IEnumerable<BatchProjection> GetBatches(int classId)
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                           where batch.ClassId == classId
                            select new BatchProjection
                            {
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                                InTime = batch.InTime,
                                OutTime = batch.OutTime,
                               ClassName = batch.Classes.Name
                            }).ToArray());
        }

        public IEnumerable<BatchProjection> GetAllBatchByPrefix(string prefix)
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from b in batches
                            where b.Name.StartsWith(prefix)
                            select new BatchProjection
                            {
                                BatchName = b.Name
                            }).ToArray());
        }

        public IEnumerable<BatchProjection> GetAllBatches()
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                            select new BatchProjection
                            {
                                ClassId = batch.ClassId,
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                                InTime = batch.InTime,
                                OutTime = batch.OutTime,
                                ClassName = batch.Classes.Name
                            }).ToArray());
        }

        public CMSResult Save(Batch newBatch)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Batch, bool>(batches => (from batch in batches where batch.Name == newBatch.Name && batch.ClassId == newBatch.ClassId select batch).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Batch '{0}' already exists!", newBatch.Name) });
            }
            else
            {
                _repository.Add(newBatch);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Batch '{0}' successfully added!", newBatch.Name) });
            }
            return result;
        }

        public CMSResult Save(int ClientId,Batch newBatch)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Batch, bool>(batches => (from batch in batches where batch.Name == newBatch.Name && batch.ClassId == newBatch.ClassId && batch.ClientId== ClientId select batch).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Batch '{0}' already exists!", newBatch.Name) });
            }
            else
            {
                newBatch.ClientId = ClientId;
                _repository.Add(newBatch);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Batch '{0}' successfully added!", newBatch.Name) });
            }
            return result;
        }

        public CMSResult Update(Batch oldBatch)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Batch, bool>(batches => (from batch in batches where batch.BatchId != oldBatch.BatchId && batch.ClassId == oldBatch.ClassId && batch.Name == oldBatch.Name select batch).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Batch '{0}' already exists!", oldBatch.Name) });
            }
            else
            {

                var btch = _repository.Load<Batch>(x => x.BatchId == oldBatch.BatchId);
              //  btch.ClassId = oldBatch.ClassId;
                btch.Name = oldBatch.Name;
                btch.InTime = oldBatch.InTime;
                btch.OutTime = oldBatch.OutTime;
                _repository.Update(btch);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Batch '{0}' successfully updated!", oldBatch.Name) });
            }
            return result;
        }

        public CMSResult Delete(int BatchId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Batch>(x => x.BatchId == BatchId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Batch '{0}' does not already exists!", model.Name) });
            }
            else
            {
                var isExistsStudent = _repository.Project<Student, bool>(students => (
                                            from s in students
                                            where (s.BatchId==BatchId)
                                            select s)
                                            .Any());

                var isExistsAttendance = _repository.Project<Attendance, bool>(attendances => (
                                            from a in attendances
                                            where a.BatchId == BatchId
                                            select a)
                                            .Any());

                if (isExistsStudent || isExistsAttendance)
                {
                    var selectModel = "";
                    selectModel += (isExistsStudent) ? "Student, " : "";
                    selectModel += (isExistsAttendance) ? "Attendance, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Batch '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Batch '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public BatchProjection GetBatcheById(int batchId)
        {
            return _repository.Project<Batch, BatchProjection>(
                batches => (from batch in batches
                            where batch.BatchId == batchId
                            
                            select new BatchProjection
                            {
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                                ClassName = batch.Classes.Name,
                                InTime = batch.InTime,
                                OutTime = batch.OutTime,
                               ClassId = batch.ClassId
                            }).FirstOrDefault());
        }

        public IEnumerable<BatchProjection> GetAllBatchesByClassId(int classId)
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                            where batch.ClassId == classId
                            select new BatchProjection
                            {
                                ClassId = batch.ClassId,
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                            }).ToArray());
        }

        public IEnumerable<BatchProjection> GetAllBatchClsId(int classId)
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                            where batch.ClassId == classId
                            select new BatchProjection
                            {
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                            }).ToArray());
        }

        public IEnumerable<BatchProjection> GetBatchesByClassId(int classId)
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                            where batch.ClassId == classId
                            select new BatchProjection
                            {
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                                InTime = batch.InTime,
                                OutTime = batch.OutTime
                            }).ToArray());
        }

        public IEnumerable<BatchProjection> GetBatchesByClassIds(string selectedClasses)
        {
            var classIds = selectedClasses.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                           where classIds.Contains(batch.ClassId)
                            select new BatchProjection
                            {
                                ClassId = batch.ClassId,
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                                ClassName = batch.Classes.Name
                            }).ToArray());
        }

        public int GetBatchesCount()
        {
            return _repository.Project<Batch, int>(
               batches => (from batch in batches select batch).Count());
        }

        public int GetBatchesCountByClientId(int ClientId)
        {
            return _repository.Project<Batch, int>(
               batches => (from batch in batches where batch.ClientId==ClientId select batch).Count());
        }

       



        public IEnumerable<BatchGridModel> GetBatchData(out int totalRecords, int filterClassName,
         int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Batch, IQueryable<BatchGridModel>>(Batches => (
                 from b in Batches
                 select new BatchGridModel
                 {
                     BatchId = b.BatchId,
                     BatchName = b.Name,
                     ClassName = b.Classes.Name,
                     InTime = b.InTime,
                     OutTime = b.OutTime,
                     ClassId=b.Classes.ClassId,
                      CreatedOn = b.CreatedOn,
                 })).AsQueryable();

            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BatchGridModel.BatchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BatchName);
                        else
                            query = query.OrderByDescending(p => p.BatchName);
                        break;
                    case nameof(BatchGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }
            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }
            return query.ToList();
        }


        public IEnumerable<BatchGridModel> GetBatchDataByClientId(out int totalRecords, int filterClassName, int ClientId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Batch, IQueryable<BatchGridModel>>(Batches => (
                 from b in Batches
                 where b.ClientId == ClientId
                 select new BatchGridModel
                 {
                     BatchId = b.BatchId,
                     BatchName = b.Name,
                     ClassName = b.Classes.Name,
                     InTime = b.InTime,
                     OutTime = b.OutTime,
                     ClassId = b.Classes.ClassId,
                     CreatedOn = b.CreatedOn,
                 })).AsQueryable();

            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName  );
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BatchGridModel.BatchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BatchName);
                        else
                            query = query.OrderByDescending(p => p.BatchName);
                        break;
                    case nameof(BatchGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }
            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }
            return query.ToList();
        }


        public IEnumerable<BatchProjection> GetBatchesByBatchIds(string SelectedBatches)
        {
            var batchIds = SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                            where batchIds.Contains(batch.BatchId)
                            select new BatchProjection
                            {
                                BatchName = batch.Name,
                                ClassName = batch.Classes.Name
                            }).ToArray());
        }

        public IEnumerable<BatchProjection> GetBatchesBySubjectId(int classId)
        {
            return _repository.Project<Batch, BatchProjection[]>(
                batches => (from batch in batches
                            where batch.ClassId == classId
                            select new BatchProjection
                            {
                                BatchId = batch.BatchId,
                                BatchName = batch.Name,
                                InTime = batch.InTime,
                                OutTime = batch.OutTime,
                              //  ClassName = batch.Subject.Class.Name
                            }).ToArray());
        }
    }
}
