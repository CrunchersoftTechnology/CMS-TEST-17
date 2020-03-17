using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadNotesService : IUploadNotesService
    {
        readonly IRepository _repository;

        public UploadNotesService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(int ClientId,UploadNotes newUploadNotes)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadNotes, bool>(uploadNotes => (
                                from p in uploadNotes
                                where p.FileName == newUploadNotes.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Notes file '{0}' already exists!", newUploadNotes.FileName) });
            }
            else
            {
                newUploadNotes.ClientId = ClientId;
                _repository.Add(newUploadNotes);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Notes file '{0}' added successfully!", newUploadNotes.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadNotesGridModel> GetUploadNotesData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadNotes, IQueryable<UploadNotesGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadNotesGridModel
                 {
                     UploadNotesId = p.UploadNotesId,
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
                    case nameof(UploadNotesGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadNotesGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadNotesGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadNotesGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadNotesGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadNotesGridModel.IsVisible):
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

        public UploadNotesProjection GetNotesById(int uploadNotesId)
        {
            return _repository.Project<UploadNotes, UploadNotesProjection>(
                    uploadNotes => (from notes in uploadNotes
                                    where notes.UploadNotesId == uploadNotesId
                                    select new UploadNotesProjection
                                    {
                                        UploadNotesId = notes.UploadNotesId,
                                        BoardName = notes.BoardName,
                                        ClassName = notes.ClassName,
                                        SubjectName = notes.SubjectName,
                                        FileName = notes.FileName,
                                        LogoName = notes.LogoName,
                                        UploadDate = notes.UploadDate,
                                        Title = notes.Title,
                                        IsVisible = notes.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadNotes uploadNewNotes)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadNotes, bool>(uploadNotes => (
                                from p in uploadNotes
                                where p.FileName == uploadNewNotes.FileName && p.UploadNotesId != uploadNewNotes.UploadNotesId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Notes file '{0}' already exists!", uploadNewNotes.FileName) });
            }
            else
            {
                var notes = _repository.Load<UploadNotes>(x => x.UploadNotesId == uploadNewNotes.UploadNotesId);
                notes.ClassName = uploadNewNotes.ClassName;
                notes.Title = uploadNewNotes.Title;
                notes.FileName = uploadNewNotes.FileName;
                notes.LogoName = uploadNewNotes.LogoName;
                notes.BoardName = uploadNewNotes.BoardName;
                notes.SubjectName = uploadNewNotes.SubjectName;
                notes.UploadDate = uploadNewNotes.UploadDate;
                notes.IsVisible = uploadNewNotes.IsVisible;
                _repository.Update(notes);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Notes updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadNotesId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadNotes>(b => b.UploadNotesId == uploadNotesId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Notes not exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Notes deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadNotesProjection> GetUploadNotesList()
        {
            return _repository.Project<UploadNotes, UploadNotesProjection[]>(
                UploadNotes => (from notes in UploadNotes
                                where notes.IsVisible == true
                                orderby notes.CreatedOn descending
                                select new UploadNotesProjection
                                {
                                    BoardName = notes.BoardName,
                                    ClassName = notes.ClassName,
                                    SubjectName = notes.SubjectName,
                                    Title = notes.Title,
                                    UploadDate = notes.UploadDate,
                                    FileName = notes.FileName,
                                    LogoName = notes.LogoName,
                                    IsVisible = notes.IsVisible,
                                    UploadNotesId = notes.UploadNotesId,
                                    ClassId = notes.ClassId,
                                    BoardId = notes.BoardId,
                                    SubjectId = notes.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadNotesProjection> GetUploadNotesListByClientId(int ClientId)
        {
            return _repository.Project<UploadNotes, UploadNotesProjection[]>(
                UploadNotes => (from notes in UploadNotes
                                where notes.IsVisible == true
                                && notes.ClientId==ClientId
                                orderby notes.CreatedOn descending
                                select new UploadNotesProjection
                                {
                                    BoardName = notes.BoardName,
                                    ClassName = notes.ClassName,
                                    SubjectName = notes.SubjectName,
                                    Title = notes.Title,
                                    UploadDate = notes.UploadDate,
                                    FileName = notes.FileName,
                                    LogoName = notes.LogoName,
                                    IsVisible = notes.IsVisible,
                                    UploadNotesId = notes.UploadNotesId,
                                    ClassId = notes.ClassId,
                                    BoardId = notes.BoardId,
                                    SubjectId = notes.SubjectId
                                }).ToArray());
        }


        public IEnumerable<UploadNotesProjection> GetUploadNotesListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadNotes, UploadNotesProjection[]>(
                UploadNotes => (from notes in UploadNotes
                                where notes.IsVisible == true && notes.SubjectId == subjectId
                                orderby notes.CreatedOn descending
                                select new UploadNotesProjection
                                {
                                    Title = notes.Title,
                                    UploadNotesId = notes.UploadNotesId
                                }).ToArray());
        }

    }
}
