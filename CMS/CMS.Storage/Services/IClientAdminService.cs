using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IClientAdminService
    {
        CMSResult Save(ClientAdmin clientAdmin);
        ClientAdminProjection GetClientAdminById(string clientAdminId);
        CMSResult Update(ClientAdmin clientAdmin);
        IEnumerable<ClientAdminProjection> GetClients();
        CMSResult Delete(string userId);
        ClientAdminProjection GetClientsById(string ClientAdminId);
        IEnumerable<ClientAdminProjection> GetClientAdminContactList();
        IEnumerable<ClientAdminGridModel> GetClientAdminData(out int totalRecords, string Name,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<ClientAdminProjection> GetClientAdminContactListBrclientId(int clientId);
    }
}
