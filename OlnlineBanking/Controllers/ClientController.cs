using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IClientRepository _clientRepository;
        public int PageSize = 7;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        
        public ActionResult Index(int page=1, string sortedField ="", Ordering sortedOrder=Ordering.Asc, ClientStatus? statusFilter = null)
        {
            PagingInfo pagingInfo = MakePagingInfo(page);
            SortedInfo sortedInfo = MakeSortedInfo(sortedField, sortedOrder);
            ClientListViewModel clientListViewModel = MakeClientListViewModel(pagingInfo, sortedInfo, statusFilter);
            return View(clientListViewModel);
        }


        private PagingInfo MakePagingInfo(int page=1)
        {
            PagingInfo pagingInfo = new PagingInfo()
            {
                ItemsPerPage = PageSize,
                TotalItems = _clientRepository.Clients.Count(),
                CurrentPage = page,
            };
            return pagingInfo;
        }
        
        private SortedInfo MakeSortedInfo(string sortedField = "", Ordering sortedOrder = Ordering.Asc)
        {
            SortedInfo sortedInfo = new SortedInfo()
            {
                SortedField = string.IsNullOrEmpty(sortedField) ? "ContractNumber" : sortedField,
                SortedOrder = sortedOrder
            };
            return sortedInfo;
        }

        private ClientListViewModel MakeClientListViewModel(PagingInfo pagingInfo, SortedInfo sortedInfo, ClientStatus? statusFilter)
        {
            //we should form linq query with sorted info and paging info 
            Func<Client,string> orderByField = client => client.ContractNumber;
            switch (sortedInfo.SortedField.ToLower())
            {
                case "contractnumber":
                    orderByField = client => client.ContractNumber;
                    break;
                case "firstname":
                    orderByField = client => client.FirstName;
                    break;
                case "lastname":
                    orderByField = client => client.LastName;
                    break;
                case "dateofbirth":
                    orderByField = client => client.DateOfBirth.ToShortDateString();
                    break;
                case "phone":
                    orderByField = client => client.Phone;
                    break;
                case "status":
                    orderByField = client => client.Status.ToString();
                    break;
                case "deposit":
                    orderByField = client => client.Deposit.ToString();
                    break;
                default:
                    orderByField = client => client.ContractNumber;
                    break;
            }
            IEnumerable<Client> clients = _clientRepository.Clients;

            clients = sortedInfo.SortedOrder == Ordering.Asc ? _clientRepository.Clients.OrderBy(orderByField) : _clientRepository.Clients.OrderByDescending(orderByField);
            if (statusFilter.HasValue) clients = clients.Where(c => c.Status == statusFilter);
        
            //we need reassign pagingInfo.TotalItems because of filters
            pagingInfo.TotalItems = clients.Count();

            clients = clients.Skip((pagingInfo.CurrentPage - 1)*pagingInfo.ItemsPerPage).Take(pagingInfo.ItemsPerPage);
   
            ClientListViewModel clientListViewModel = new ClientListViewModel()
            {
                PagingInfo = pagingInfo,
                SortedInfo = sortedInfo,
                StatusFilter = statusFilter,
                Clients = clients
            };
            return clientListViewModel;
        }

        public ActionResult Edit(int id, string returnUrl="")
        {
            ViewBag.ReturnUrl = returnUrl;
            Client client = _clientRepository.Clients.FirstOrDefault(c => c.Id == id);
            return View(client);
        }

        public ActionResult Add(string returnUrl="")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Edit",new Client()
            {
                DateOfBirth = new DateTime(DateTime.Now.AddYears(-30).Year,1,1)
            });
        }

        [HttpPost]
        public ActionResult Edit(Client client, string returnUrl="")
        {
            if (ModelState.IsValid)
            {
                int clientEditId =_clientRepository.SaveClient(client);
                return Redirect(string.IsNullOrEmpty(returnUrl)? Url.Action("Index"):returnUrl);
            }
            else
            {
                return View("Edit",client);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl="")
        {
            _clientRepository.DeleteClient(id);
            return Redirect(string.IsNullOrEmpty(returnUrl) ? Url.Action("Index") : returnUrl);
        }
    }
}