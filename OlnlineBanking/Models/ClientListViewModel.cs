using System.Collections.Generic;

namespace OlnlineBanking.Models
{
    public class ClientListViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public SortedInfo SortedInfo { get; set; }
        public ClientStatus? StatusFilter { get; set; }
        public IEnumerable<Client> Clients { get; set; } 
    }
}