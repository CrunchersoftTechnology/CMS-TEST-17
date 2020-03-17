using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;


namespace CMS.Domain.Storage.Services
{
    public interface IConfigureServices
    {
        CMSResult Save(int ClientId, Configure configure);
        CMSResult Update(Configure configure);
        ConfigureProjection GetConfigureById(int clientId);
        ConfigureProjection GetConfById(int clientId);
        IEnumerable<ConfigureProjection> GetAllConfigure();
        int GetConfigureCount();
        IEnumerable<ConfigureGridModel> GetConfigureData(out int totalRecords, string Name,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc,int userid);
        IEnumerable<ConfigureProjection> GetConfigureByMultipleConfigureId(string selectedClient);

    }
}
