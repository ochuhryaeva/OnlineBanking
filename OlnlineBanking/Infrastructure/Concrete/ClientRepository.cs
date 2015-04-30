using System.Collections.Generic;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ClientRepository:IClientRepository
    {
        private readonly ClientDbContext _context;

        public ClientRepository(ClientDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Client> Clients
        {
            get
            {
                return _context.Clients ;
            }
        }

        public int SaveClient(Client client)
        {
            if (client.Id == 0)
            {
                _context.Clients.Add(client);
            }
            else
            {
                Client clientEntry = _context.Clients.Find(client.Id);
                if (clientEntry != null)
                {
                    UpdateClientEntry(clientEntry,client);   
                }
            }
            _context.SaveChanges();
            return client.Id;
        }

        private void UpdateClientEntry(Client clientTarget, Client clientSource)
        {
            foreach (var property in clientSource.GetType().GetProperties())
            {
                var value = clientSource.GetType().GetProperty(property.Name).GetValue(clientSource);
                clientTarget.GetType().GetProperty(property.Name).SetValue(clientTarget, value);
            }
        }

        public void DeleteClient(int clientId)
        {
            Client clientEntry = _context.Clients.Find(clientId);
            if (clientEntry != null)
            {
                _context.Clients.Remove(clientEntry);
                _context.SaveChanges();
            }
        }
    }
}
