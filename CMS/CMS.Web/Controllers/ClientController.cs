using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class ClientController : BaseController
    {
        readonly IClientService _clientService;
        readonly ILogger _logger;
        readonly IRepository _repository;

        public ClientController(IClientService clientService, ILogger logger, IRepository repository)
        {
            _clientService = clientService;
            _logger = logger;
            _repository = repository;
        }

    
        public ActionResult Index()
        {
           
            return View();
        }

      
        public ActionResult Details(int id)
        {
            return View();
        }

      
        public ActionResult Create()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var client = new Client
                {
                    Address = viewModel.Address,
                    Name = viewModel.Name
                };

                var result = _clientService.Save(client);
                if (result.Success)
                {

                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }

            viewModel = new ClientViewModel();

            return View(viewModel);
        }

        // GET: Branch/Edit/5
        public ActionResult Edit(int id)
        {
            var projection = _clientService.GetBoardById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Client does not Exists {0}.", id));
                Warning("Client does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<ClientProjection, ClientViewModel>(projection);
            return View(viewModel);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var board = _repository.Project<Client, bool>(clients => (from b in clients where b.ClientId == viewModel.ClientId select b).Any());
                if (!board)
                {
                    _logger.Warn(string.Format("Clients not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Clients not exists '{0}'.", viewModel.Name));
                }
                var result = _clientService.Update(new Client
                {
                    Name = viewModel.Name,
                    Address = viewModel.Address,
                    ClientId = viewModel.ClientId
                });
                if (result.Success)
                {
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            return View(viewModel);
        }

        // GET: Branch/Delete/5
        public ActionResult Delete(int id)
        {
            var projection = _clientService.GetBoardById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Board does not Exists {0}.", id));
                Warning("Board does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<ClientProjection, ClientViewModel>(projection);
            return View(viewModel);
        }

        // POST: Branch/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ClientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _clientService.Delete(viewModel.ClientId);
                if (result.Success)
                {
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
