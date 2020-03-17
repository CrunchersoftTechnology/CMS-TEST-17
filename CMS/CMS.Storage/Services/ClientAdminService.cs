using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using System.Linq;
using System;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class ClientAdminService : IClientAdminService
    {
        readonly IRepository _repository;

        public ClientAdminService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(ClientAdmin clientAdmin)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Client, bool>(clients => (from b in clients where b.Name == clientAdmin.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Client '{0}' already exists!", clientAdmin.Name) });
            }
            else
            {
                _repository.Add(clientAdmin);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Client '{0}' successfully added!", clientAdmin.Name) });
            }
            return result;
        }

        public ClientAdminProjection GetClientAdminById(string clientAdminId)
        {
            return _repository.Project<ClientAdmin, ClientAdminProjection>(
                clientAdmins => (from b in clientAdmins
                                 where b.UserId == clientAdminId                              
                                 select new ClientAdminProjection
                                 {
                                     ClientId = b.ClientId,
                                     ClientName = b.Client.Name,
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     Name = b.Name,
                                     UserId = b.UserId,
                                     Active = b.Active
                                 }).FirstOrDefault());
        }

        public CMSResult Update(ClientAdmin clientAdmin)
        {
            var result = new CMSResult();
            var batch = _repository.Project<ClientAdmin, bool>(users => (from u in users where u.UserId == clientAdmin.UserId select u).Any());

            if (!batch)
            {
                result.Results.Add(new Result { Message = "Client Admin not exists." });
            }
            else
            {
                var clientAdminContact = _repository.Project<ClientAdmin, bool>(clientAdmins => (from a in clientAdmins where a.ContactNo == clientAdmin.ContactNo && a.UserId != clientAdmin.UserId select a).Any());
                if (clientAdminContact)
                {
                    result.Results.Add(
                        new Result
                        {
                            IsSuccessful = false,
                            Message = string.Format("Contact Number already exists!")
                        });
                }

                if (!clientAdminContact)
                {
                    var ClientAdminUser = _repository.Load<ApplicationUser>(x => x.Id == clientAdmin.UserId, x => x.ClientAdmin);

                    ClientAdminUser.ClientAdmin.ClientId = clientAdmin.ClientId;
                    ClientAdminUser.ClientAdmin.Name = clientAdmin.Name;
                    ClientAdminUser.ClientAdmin.ContactNo = clientAdmin.ContactNo;
                    ClientAdminUser.ClientAdmin.Active = clientAdmin.Active;

                    _repository.Update(ClientAdminUser);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Client Admin '{0}' successfully updated!", clientAdmin.Name) });
                }
            }

            return result;
        }

        public IEnumerable<ClientAdminProjection> GetClients()
        {
            return _repository.Project<ClientAdmin, ClientAdminProjection[]>(
                clientAdmins => (from b in clientAdmins
                                 orderby b.Name
                                 select new ClientAdminProjection
                                 {
                                     ClientName = b.Client.Name,
                                     Name = b.Name,
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     Active = b.Active,
                                     UserId = b.UserId,
                                     AId = b.AId
                                 }).ToArray());
        }

        public CMSResult Delete(string userId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<ClientAdmin>(c => c.UserId == userId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Client Admin '{0}' already exists!", model.Name) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("client Admin '{0}' deleted successfully!", model.Name) });

            }
            return result;
        }

        public ClientAdminProjection GetClientsById(string clientAdminId)
        {
            return _repository.Project<ClientAdmin, ClientAdminProjection>(
                clientAdmins => (from b in clientAdmins
                                 where b.UserId == clientAdminId
                                 select new ClientAdminProjection
                                 {
                                     ClientName = b.Client.Name,
                                     ClientId = b.ClientId
                                 }).FirstOrDefault());
        }

        public IEnumerable<ClientAdminProjection> GetClientAdminContactList()
        {
            return _repository.Project<ClientAdmin, ClientAdminProjection[]>(
                clientAdmins => (from b in clientAdmins
                                 where b.Active == true
                                 select new ClientAdminProjection
                                 {
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     ClientId = b.ClientId,
                                     Name = b.Name,

                                 }).ToArray());
        }

        public IEnumerable<ClientAdminGridModel> GetClientAdminData(out int totalRecords, string Name,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<ClientAdmin, IQueryable<ClientAdminGridModel>>(clientAdmins => (
                 from b in clientAdmins
                 select new ClientAdminGridModel
                 {
                     AId = b.AId,
                     Name = b.Name,
                     ContactNo = b.ContactNo,
                     Email = b.User.Email,
                     ClientName = b.Client.Name,
                     IsActive = b.Active,
                     UserId = b.UserId,
                     CreatedOn = b.CreatedOn,
                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.Name.Contains(Name));
            }
            //if (!string.IsNullOrWhiteSpace(globalSearch))
            //{
            //    query = query.Where(p => (p.FirstName + " " + p.LastName).Contains(globalSearch));
            //}

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ClientAdminGridModel.Name):
                        if (!desc)
                            query = query.OrderBy(p => p.Name);
                        else
                            query = query.OrderByDescending(p => p.Name);
                        break;
                    case nameof(ClientAdminGridModel.ClientName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClientName);
                        else
                            query = query.OrderByDescending(p => p.ClientName);
                        break;
                    case nameof(ClientAdminGridModel.IsActive):
                        if (!desc)
                            query = query.OrderBy(p => p.IsActive);
                        else
                            query = query.OrderByDescending(p => p.IsActive);
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

        public IEnumerable<ClientAdminProjection> GetClientAdminContactListBrclientId(int clientId)
        {
            return _repository.Project<ClientAdmin, ClientAdminProjection[]>(
                clientAdmins => (from b in clientAdmins
                                 where b.Active == true && b.ClientId == clientId
                                 select new ClientAdminProjection
                                 {
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     ClientId = b.ClientId,
                                     Name = b.Name,

                                 }).ToArray());
        }
    }
}
