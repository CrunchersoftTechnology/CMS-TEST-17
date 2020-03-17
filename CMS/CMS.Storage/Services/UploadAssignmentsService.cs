using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadAssignmentsService : IUploadAssignmentsService
    {
        readonly IRepository _repository;

        public UploadAssignmentsService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(UploadAssignments newUploadAssignments)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadAssignments, bool>(uploadAssignments => (
                                from p in uploadAssignments
                                where p.FileName == newUploadAssignments.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Assignments file '{0}' already exists!", newUploadAssignments.FileName) });
            }
            else
            {
                _repository.Add(newUploadAssignments);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Assignments file '{0}' added successfully!", newUploadAssignments.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadAssignmentsGridModel> GetUploadAssignmentsData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadAssignments, IQueryable<UploadAssignmentsGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadAssignmentsGridModel
                 {
                     UploadAssignmentsId = p.UploadAssignmentsId,
                     Title = p.Title,
                     BoardName = p.BoardName,
                     ClassName = p.ClassName,
                     SubjectName = p.SubjectName,
                     UploadDate = p.UploadDate,
                     FileName = p.FileName,
                     LogoName = p.LogoName,
                     IsVisible = p.IsVisible,
                     CreatedOn = p.CreatedOn
                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(UploadAssignmentsGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadAssignmentsGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadAssignmentsGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadAssignmentsGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadAssignmentsGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadAssignmentsGridModel.IsVisible):
                        if (!desc)
                            query = query.OrderBy(p => p.IsVisible);
                        else
                            query = query.OrderByDescending(p => p.IsVisible);
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

        public UploadAssignmentsProjection GetAssignmentsById(int uploadAssignmentsId)
        {
            return _repository.Project<UploadAssignments, UploadAssignmentsProjection>(
                    uploadAssignments => (from Assignments in uploadAssignments
                                    where Assignments.UploadAssignmentsId == uploadAssignmentsId
                                    select new UploadAssignmentsProjection
                                    {
                                        UploadAssignmentsId = Assignments.UploadAssignmentsId,
                                        BoardName = Assignments.BoardName,
                                        ClassName = Assignments.ClassName,
                                        SubjectName = Assignments.SubjectName,
                                        FileName = Assignments.FileName,
                                        LogoName = Assignments.LogoName,
                                        UploadDate = Assignments.UploadDate,
                                        Title = Assignments.Title,
                                        IsVisible = Assignments.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadAssignments uploadNewAssignments)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadAssignments, bool>(uploadAssignments => (
                                from p in uploadAssignments
                                where p.FileName == uploadNewAssignments.FileName && p.UploadAssignmentsId != uploadNewAssignments.UploadAssignmentsId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Assignments file '{0}' already exists!", uploadNewAssignments.FileName) });
            }
            else
            {
                var Assignments = _repository.Load<UploadAssignments>(x => x.UploadAssignmentsId == uploadNewAssignments.UploadAssignmentsId);
                Assignments.ClassName = uploadNewAssignments.ClassName;
                Assignments.Title = uploadNewAssignments.Title;
                Assignments.FileName = uploadNewAssignments.FileName;
                Assignments.LogoName = uploadNewAssignments.LogoName;
                Assignments.BoardName = uploadNewAssignments.BoardName;
                Assignments.SubjectName = uploadNewAssignments.SubjectName;
                Assignments.UploadDate = uploadNewAssignments.UploadDate;
                Assignments.IsVisible = uploadNewAssignments.IsVisible;
                _repository.Update(Assignments);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Assignments updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadAssignmentsId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadAssignments>(b => b.UploadAssignmentsId == uploadAssignmentsId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Assignments not exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Assignments deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadAssignmentsProjection> GetUploadAssignmentsList()
        {
            return _repository.Project<UploadAssignments, UploadAssignmentsProjection[]>(
                UploadAssignments => (from Assignments in UploadAssignments
                                      where Assignments.IsVisible == true
                                orderby Assignments.CreatedOn descending
                                select new UploadAssignmentsProjection
                                {
                                    BoardName = Assignments.BoardName,
                                    ClassName = Assignments.ClassName,
                                    SubjectName = Assignments.SubjectName,
                                    Title = Assignments.Title,
                                    UploadDate = Assignments.UploadDate,
                                    FileName = Assignments.FileName,
                                    LogoName = Assignments.LogoName,
                                    IsVisible = Assignments.IsVisible,
                                    UploadAssignmentsId = Assignments.UploadAssignmentsId,
                                    ClassId = Assignments.ClassId,
                                    BoardId = Assignments.BoardId,
                                    SubjectId = Assignments.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadAssignmentsProjection> GetUploadAssignmentsListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadAssignments, UploadAssignmentsProjection[]>(
                UploadAssignments => (from Assignments in UploadAssignments
                                where Assignments.IsVisible == true && Assignments.SubjectId == subjectId
                                orderby Assignments.CreatedOn descending
                                select new UploadAssignmentsProjection
                                {
                                    Title = Assignments.Title,
                                    UploadAssignmentsId = Assignments.UploadAssignmentsId
                                }).ToArray());
        }

    }
}
