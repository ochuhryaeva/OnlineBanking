using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OlnlineBanking.Models
{
    public class ClientListViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public SortedInfo SortedInfo { get; set; }
        public IEnumerable<Client> Clients { get; set; } 
    }
}