using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Abstract
{
    public interface IClientRepository
    {
        IEnumerable<Client> Clients { get; }
        int SaveClient(Client client);
        void DeleteClient(int clientId);
    }
}
