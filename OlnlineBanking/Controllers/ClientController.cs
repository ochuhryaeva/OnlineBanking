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
        public int PageSize = 7;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        
        //[Authorize]
        public ActionResult Index(int page=1, int selectedClientId = 0)
        {
            int pageToShow = page;
            //for selectedClientId !=0 need to find page with this id and show it
            if (selectedClientId != 0) pageToShow = PageNumberForClientId(selectedClientId);
            PagingInfo pagingInfo = new PagingInfo()
            {
                ItemsPerPage = PageSize,
                TotalItems = _clientRepository.Clients.Count(),
                CurrentPage = pageToShow,
            };
            ClientListViewModel clientListViewModel = new ClientListViewModel()
            {
                PagingInfo = pagingInfo,
                Clients =
                    _clientRepository.Clients.OrderBy(c => c.ContractNumber).Skip((pagingInfo.CurrentPage - 1)*PageSize).Take(PageSize)
            }; 
            
            return View(clientListViewModel);
        }

        private int PageNumberForClientId(int selectedClientId)
        {
            int page = 1;
            var clientWithPositions = _clientRepository.Clients
                                        .OrderBy(c => c.ContractNumber)
                                        .Select((c, i) => new { clientPos = i + 1, clientId = c.Id });
            var selectedClientWithPosition = clientWithPositions.FirstOrDefault(c => c.clientId == selectedClientId);
            if (selectedClientWithPosition != null)
            {
                page = (int)Math.Ceiling((decimal)selectedClientWithPosition.clientPos / PageSize);
            }
            return page;
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
                int clientEditId =_clientRepository.SaveClient(client);
                return RedirectToAction("Index",new {selectedClientId=clientEditId});
            }
            else
            {
                return View("Edit",client);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, int currentPage=1)
        {
            _clientRepository.DeleteClient(id);
            int totalPages = (int) Math.Ceiling((decimal) _clientRepository.Clients.Count()/PageSize);
            if (totalPages > 1 && totalPages < currentPage) currentPage--;
            return RedirectToAction("Index",new {page=currentPage});
        }
    }
}