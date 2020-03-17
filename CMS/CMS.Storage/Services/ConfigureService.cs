using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Linq;
using CMS.Common;
using CMS.Common.GridModels;
using System;
using System.Collections.Generic;
using CMS.Domain.Infrastructure;

namespace CMS.Domain.Storage.Services
{
    public class ConfigureService : IConfigureServices
    {
        readonly IRepository _repository;

        public ConfigureService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(int ClientId, Configure newconfigure)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Configure, bool>(configure => (
                                from b in configure
                                where b.name == newconfigure.name && b.ConfigureId == newconfigure.ConfigureId
                                select b
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Configure '{0}' already exists!", newconfigure.name) });
            }
            else
            {
                newconfigure.ClientId = ClientId;
                _repository.Add(newconfigure);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Configure '{0}' successfully added!", newconfigure.name) });
            }
            return result;
        }

        public CMSResult Update(Configure configure)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Configure, bool>(clients => (from b in clients where b.ConfigureId != configure.ConfigureId && b.name == configure.name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Configure '{0}' already exists!", configure.name) });
            }
            else
            {
                var brch = _repository.Load<Configure>(x => x.ConfigureId == configure.ConfigureId);

                brch.name = configure.name;
                brch.address = configure.address;
                brch.email_id = configure.email_id;
                brch.confirmemail = configure.confirmemail;
                brch.emailpassword = configure.emailpassword;
                brch.username = configure.username;
                brch.aboutus = configure.aboutus;
                brch.sender_id = configure.sender_id;
                brch.password = configure.password;
                brch.websiteURL = configure.websiteURL;
                _repository.Update(brch);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("configure '{0}' successfully updated!", configure.name) });

            }
            return result;
        }

        public IEnumerable<ConfigureProjection> GetAllConfigure()
        {
            return _repository.Project<Configure, ConfigureProjection[]>(
                Configures => (from c in Configures
                               select new ConfigureProjection
                               {
                                   ConfigureId = c.ConfigureId,
                                   name = c.name,
                                   address = c.address,
                                   email_id = c.email_id,
                                   confirmemail = c.confirmemail,
                                   emailpassword = c.emailpassword,
                                   username = c.username,
                                   aboutus = c.aboutus,
                                   sender_id = c.sender_id,
                                   password = c.password,
                                   websiteURL = c.websiteURL
                               }).ToArray());
        }

        public int GetConfigureCount()
        {
            return _repository.Project<Configure, int>(
             configures => (from configure in configures select configure).Count());
        }

        public ConfigureProjection GetConfigureById(int clientId)
        {
            return _repository.Project<Configure, ConfigureProjection>(
                Configure => (from c in Configure
                              where c.ClientId == clientId
                              select new ConfigureProjection
                              {
                                  ConfigureId = c.ConfigureId,
                                  name = c.name,
                                  address = c.address,
                                  email_id = c.email_id,
                                  confirmemail = c.confirmemail,
                                  emailpassword = c.emailpassword,
                                  username = c.username,
                                  aboutus = c.aboutus,
                                  sender_id = c.sender_id,
                                  password = c.password,
                                  websiteURL = c.websiteURL

                              }).FirstOrDefault());
        }
        public ConfigureProjection GetConfById(int configureId)
        {
            return _repository.Project<Configure, ConfigureProjection>(
                branchAdmins => (from c in branchAdmins
                                 where
                                c.ConfigureId == configureId
                                 select new ConfigureProjection
                                 {
                                     ConfigureId = c.ConfigureId,
                                     name = c.name,
                                     address = c.address,
                                     email_id = c.email_id,
                                     confirmemail = c.confirmemail,
                                     emailpassword = c.emailpassword,
                                     username = c.username,
                                     aboutus = c.aboutus,
                                     sender_id = c.sender_id,
                                     password = c.password,
                                     websiteURL = c.websiteURL
                                 }).FirstOrDefault());
        }

        IEnumerable<ConfigureProjection> IConfigureServices.GetConfigureByMultipleConfigureId(string selectedClient)
        {
            var clientIds = selectedClient.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Configure, ConfigureProjection[]>(
                configure => (from c in configure
                              where clientIds.Contains(c.ConfigureId)
                              select new ConfigureProjection
                              {

                                  ConfigureId = c.ConfigureId,
                                  name = c.name,
                                  address = c.address,
                                  email_id = c.email_id,
                                  confirmemail = c.confirmemail,
                                  emailpassword = c.emailpassword,
                                  username = c.username,
                                  aboutus = c.aboutus,
                                  sender_id = c.sender_id,
                                  password = c.password,
                                  websiteURL = c.websiteURL
                              }).ToArray());
        }


        public IEnumerable<ConfigureGridModel> GetConfigureData(out int totalRecords, string Name,
              int? limitOffset, int? limitRowCount, string orderBy, bool desc, int userid)
        {
            var query = _repository.Project<Configure, IQueryable<ConfigureGridModel>>(Configure => (
                  from c in Configure
                  where c.ClientId == userid
                  select new ConfigureGridModel
                  {
                      ConfigureId = c.ConfigureId,
                      name = c.name,
                      address = c.address,
                      email_id = c.email_id,
                      confirmemail = c.confirmemail,
                      emailpassword = c.emailpassword,
                      username = c.username,
                      aboutus = c.aboutus,
                      sender_id = c.sender_id,
                      password = c.password,
                      websiteURL = c.websiteURL,
                      CreatedOn = c.CreatedOn

                  })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.name.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ConfigureGridModel.name):
                        if (!desc)
                            query = query.OrderBy(p => p.name);
                        else
                            query = query.OrderByDescending(p => p.name);
                        break;
                    case nameof(ConfigureGridModel.aboutus):
                        if (!desc)
                            query = query.OrderBy(p => p.aboutus);
                        else
                            query = query.OrderByDescending(p => p.aboutus);
                        break;
                    case nameof(ConfigureGridModel.email_id):
                        if (!desc)
                            query = query.OrderBy(p => p.email_id);
                        else
                            query = query.OrderByDescending(p => p.email_id);
                        break;
                    case nameof(ConfigureGridModel.confirmemail):
                        if (!desc)
                            query = query.OrderBy(p => p.confirmemail);
                        else
                            query = query.OrderByDescending(p => p.confirmemail);
                        break;
                    case nameof(ConfigureGridModel.emailpassword):
                        if (!desc)
                            query = query.OrderBy(p => p.emailpassword);
                        else
                            query = query.OrderByDescending(p => p.emailpassword);
                        break;
                    case nameof(ConfigureGridModel.username):
                        if (!desc)
                            query = query.OrderBy(p => p.username);
                        else
                            query = query.OrderByDescending(p => p.username);
                        break;
                    case nameof(ConfigureGridModel.address):
                        if (!desc)
                            query = query.OrderBy(p => p.address);
                        else
                            query = query.OrderByDescending(p => p.address);
                        break;
                    case nameof(ConfigureGridModel.sender_id):
                        if (!desc)
                            query = query.OrderBy(p => p.sender_id);
                        else
                            query = query.OrderByDescending(p => p.sender_id);
                        break;
                    case nameof(ConfigureGridModel.password):
                        if (!desc)
                            query = query.OrderBy(p => p.password);
                        else
                            query = query.OrderByDescending(p => p.password);
                        break;
                    case nameof(ConfigureGridModel.websiteURL):
                        if (!desc)
                            query = query.OrderBy(p => p.websiteURL);
                        else
                            query = query.OrderByDescending(p => p.websiteURL);
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
    }
}
