using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Services
{
    public interface IDailyPracticePaperService
    {
        CMSResult Save(DailyPracticePaper DailyPracticePaper);
        IEnumerable<DailyPracticePaperGridModel> GetDailyPracticePaper(out int totalRecords, int userId,
       int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        DailyPracticePaperProjection GetDailyPracticePaperById(int id);
        CMSResult Delete(int DailyPracticePaperId);
        CMSResult Update(DailyPracticePaper oldPaper);
 
    }
}
