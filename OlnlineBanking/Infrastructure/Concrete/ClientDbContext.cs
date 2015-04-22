using System.Data.Entity;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ClientDbContext:DbContext
    {
        public DbSet<Client> Clients { get; set; }
    }
}