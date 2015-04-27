using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ClientRepository:IClientRepository
    {
        //private ClientDbContext _context = new ClientDbContext();
        private ClientDbContext _context;

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
                    foreach (var property in client.GetType().GetProperties())
                    {
                        var value = client.GetType().GetProperty(property.Name).GetValue(client);
                        clientEntry.GetType().GetProperty(property.Name).SetValue(clientEntry,value);
                    }
                }
            }
            _context.SaveChanges();
            return client.Id;
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
