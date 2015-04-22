using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private IClientRepository _clientRepository;
        public int PageSize = 3;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        
        //[Authorize]
        public ActionResult Index(int page=1)
        {
            ClientListViewModel clientListViewModel = new ClientListViewModel()
            {
                Clients =
                    _clientRepository.Clients.OrderBy(c => c.ContractNumber).Skip((page - 1)*PageSize).Take(PageSize),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage   = page,
                    ItemsPerPage = PageSize,
                    TotalItems = _clientRepository.Clients.Count()
                }
            }; 
            
            return View(clientListViewModel);
        }

        public ActionResult Edit(int id)
        {
            Client client = _clientRepository.Clients.FirstOrDefault(c => c.Id == id);
            return View(client);
        }

        public ActionResult Add()
        {
            return View("Edit",new Client()
            {
                DateOfBirth = new DateTime(DateTime.Now.AddYears(-30).Year,1,1)
            });
        }

        [HttpPost]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                _clientRepository.SaveClient(client);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Edit",client);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _clientRepository.DeleteClient(id);
            return RedirectToAction("Index");
        }
    }
}