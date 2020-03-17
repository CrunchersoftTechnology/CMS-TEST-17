using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IClientService
    {
        CMSResult Save(Client client);
        CMSResult Update(Client client);
        CMSResult Delete(int clientId);
        IEnumerable<ClientProjection> GetAllClients();
        ClientProjection GetBoardById(int id);
        int GetClientsCount();

        int GetClientsCountByClientId(int ClientId);
        IEnumerable<ClientGridModel> GetClientData(out int totalRecords, string ClientName,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<ClientProjection> GetClientByMultipleClientId(string selectedClient);
    }
}
