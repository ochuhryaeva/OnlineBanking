using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class UserDbContext : DbContext
    {
        public virtual IDbSet<User> Users { get; set; }
    }
}
