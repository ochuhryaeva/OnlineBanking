using System.Collections.Generic;
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
