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
    public class ClientService : IClientService
    {
        readonly IRepository _repository;

        public ClientService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Delete(int clientId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Client>(x => x.ClientId == clientId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Client '{0}' does not already exists!", model.Name) });
            }
            else
            {
                var isExistsClientAdmin = _repository.Project<ClientAdmin, bool>(ClientAdmins => (
                                            from b in ClientAdmins
                                            where b.ClientId == clientId
                                            select b)
                                            .Any());

                var isExistsMachine = _repository.Project<Machine, bool>(machines => (
                                            from m in machines
                                            where m.ClientId == clientId
                                            select m)
                                            .Any());

                if (isExistsClientAdmin || isExistsMachine)
                {
                    var selectModel = "";
                    selectModel += (isExistsClientAdmin) ? "client Admin, " : "";
                    selectModel += (isExistsMachine) ? "Machine, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Client '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Client '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public IEnumerable<ClientProjection> GetAllClients()
        {
            return _repository.Project<Client, ClientProjection[]>(
                clients => (from client in clients
                             select new ClientProjection
                             {
                                 ClientId = client.ClientId,
                                 ClientName = client.Name,
                                 Address = client.Address
                             }).ToArray());
        }

        public ClientProjection GetBoardById(int id)
        {
            return _repository.Project<Client, ClientProjection>(
               clients => (from client in clients
                            where client.ClientId == id
                            select new ClientProjection
                            {
                                Address = client.Address,
                                ClientId = client.ClientId,
                                ClientName = client.Name
                            }).FirstOrDefault());
        }

        public int GetClientsCount()
        {
            return _repository.Project<Client, int>(
              clients => (from client in clients select client).Count());
        }

        public int GetClientsCountByClientId(int ClientId)
        {
            return _repository.Project<Client, int>(
              clients => (from client in clients where client.ClientId==ClientId select client).Count());
        }

        public CMSResult Save(Client client)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Client, bool>(clients => (from b in clients where b.Name == client.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Client '{0}' already exists!", client.Name) });
            }
            else
            {
                _repository.Add(client);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Client '{0}' successfully added!", client.Name) });
            }
            return result;
        }

        public CMSResult Update(Client client)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Client, bool>(clients => (from b in clients where b.ClientId != client.ClientId && b.Name == client.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Client '{0}' already exists!", client.Name) });
            }
            else
            {
                var brch = _repository.Load<Client>(x => x.ClientId == client.ClientId);
                brch.Name = client.Name;
                brch.Address = client.Address;

                if (brch.IsChangeDetected)
                {
                    _repository.Update(brch);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Client '{0}' successfully updated!", client.Name) });
                }
                else
                {
                    _repository.Detach(brch);
                    result.Results.Add(new Result { IsSuccessful = true, Message = "No Change Detected!" });
                }
            }
            return result;
        }

        public IEnumerable<ClientGridModel> GetClientData(out int totalRecords, string Name,
      int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Client, IQueryable<ClientGridModel>>(Clients => (
                 from b in Clients
                 select new ClientGridModel
                 {
                     ClientId = b.ClientId,
                     ClientName = b.Name,
                     Address = b.Address,
                     CreatedOn = b.CreatedOn,

                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.ClientName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ClientGridModel.ClientName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClientName);
                        else
                            query = query.OrderByDescending(p => p.ClientName);
                        break;
                    case nameof(ClientGridModel.Address):
                        if (!desc)
                            query = query.OrderBy(p => p.Address);
                        else
                            query = query.OrderByDescending(p => p.Address);
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

        public IEnumerable<ClientProjection> GetClientByMultipleClientId(string selectedClient)
        {
            var clientIds = selectedClient.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Client, ClientProjection[]>(
                client => (from s in client
                           where clientIds.Contains(s.ClientId)
                           select new ClientProjection
                           {
                               ClientId = s.ClientId,
                               ClientName = s.Name,
                           }).ToArray());
        }
    }
}
