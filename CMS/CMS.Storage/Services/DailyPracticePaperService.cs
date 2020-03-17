using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class DailyPracticePaperService : IDailyPracticePaperService
    {
        readonly IRepository _repository;
        public DailyPracticePaperService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(DailyPracticePaper dailyPracticePaper)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<DailyPracticePaper, bool>(dailyPracticePapers => (
                             from p in dailyPracticePapers
                             where p.Description == dailyPracticePaper.Description
                             select p
                         ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("PDF file '{0}' already exists!", dailyPracticePaper.Description) });
            }
            else
            {
                _repository.Add(dailyPracticePaper);
                _repository.CommitChanges();
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Daily Practice Paper added successfully!") });
            }
            return result;
        }

        public IEnumerable<DailyPracticePaperGridModel> GetDailyPracticePaper(out int totalRecords, int userId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            IQueryable<DailyPracticePaperGridModel> finalList = null;
            List<DailyPracticePaperGridModel> list = new List<DailyPracticePaperGridModel>();
            var query = _repository.Project<DailyPracticePaper, IQueryable<DailyPracticePaperGridModel>>(DailyPracticePaper => (
                 from n in DailyPracticePaper
                 select new DailyPracticePaperGridModel
                 {
                     DailyPracticePaperId = n.DailyPracticePaperId,
                     Description = n.Description,
                     CreatedOn = n.CreatedOn,
                     FileName = n.FileName,
                     SelectedBranches = n.SelectedBranches,
                     DailyPracticePaperDate=n.DailyPracticePaperDate,
                 })).AsQueryable();

            if (BranchId != 0)
            {
                foreach (var dailyPracticeProblem in query)
                {
                    var selectedBranchList = dailyPracticeProblem.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    if (selectedBranchList.Contains(BranchId))
                    {
                        list.Add(dailyPracticeProblem);
                    }
                }
                finalList = list.AsQueryable();
            }
            else
            {
                finalList = query;
            }

            totalRecords = finalList.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(DailyPracticePaperGridModel.Description):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Description);
                        else
                            finalList = finalList.OrderByDescending(p => p.Description);
                        break;
                    case nameof(DailyPracticePaperGridModel.FileName):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.FileName);
                        else
                            finalList = finalList.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(DailyPracticePaperGridModel.DailyPracticePaperDate):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.DailyPracticePaperDate);
                        else
                            finalList = finalList.OrderByDescending(p => p.DailyPracticePaperDate);
                        break;
                    default:
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.CreatedOn);
                        else
                            finalList = finalList.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }
            if (limitOffset.HasValue)
            {
                finalList = finalList.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }
            return finalList.ToList();
        }

        public DailyPracticePaperProjection GetDailyPracticePaperById(int id)
        {
            return _repository.Project<DailyPracticePaper, DailyPracticePaperProjection>(
                DailyPracticePaper => (from n in DailyPracticePaper
                                       where n.DailyPracticePaperId == id
                                       select new DailyPracticePaperProjection
                                       {
                                           Description = n.Description,
                                           CreatedOn = n.CreatedOn,
                                           SelectedBranches = n.SelectedBranches,
                                           SelectedBatches = n.SelectedBatches,
                                           SelectedClasses = n.SelectedClasses,
                                           FileName = n.FileName,
                                           DailyPracticePaperId = n.DailyPracticePaperId,
                                           AttachmentDescription=n.AttachmentDescription,
                                           DailyPracticePaperDate=n.DailyPracticePaperDate
                                       }).FirstOrDefault());
        }

        public CMSResult Delete(int DailyPracticePaperId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<DailyPracticePaper>(m => m.DailyPracticePaperId == DailyPracticePaperId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("'{0}' already exists!", model.Description) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("'{0}' deleted successfully!", model.Description) });
            }
            return result;
        }

        public CMSResult Update(DailyPracticePaper oldPaper)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<DailyPracticePaper, bool>(papers =>
            (from paper in papers
             where paper.DailyPracticePaperId != oldPaper.DailyPracticePaperId && paper.Description == oldPaper.Description
             select paper).Any());

            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Practice paper already exists!", "") });
            }
            else
            {

                var paper = _repository.Load<DailyPracticePaper>(x => x.DailyPracticePaperId == oldPaper.DailyPracticePaperId);
                paper.Description = oldPaper.Description;
                paper.AttachmentDescription = oldPaper.AttachmentDescription;
                paper.DailyPracticePaperDate = oldPaper.DailyPracticePaperDate;
                paper.FileName = oldPaper.FileName;
                _repository.Update(paper);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Time table successfully updated!", "") });
            }
            return result;
        }
    }
}
