using System.Data.Entity;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ClientDbContext:DbContext
    {
        public virtual IDbSet<Client> Clients { get; set; }
    }
}