using System.Data.Entity;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class UserDbContext : DbContext
    {
        public virtual IDbSet<User> Users { get; set; }
    }
}
