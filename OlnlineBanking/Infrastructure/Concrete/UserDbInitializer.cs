using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class UserDbInitializer : CreateDatabaseIfNotExists<UserDbContext>
    {
        protected override void Seed(UserDbContext db)
        {
            db.Users.Add(new User() { Id = 2, Login = "w3core", Email = "w3core@gmail.com", Password = "12345678", IsActivated = true, IsBlocked = false});
            base.Seed(db);
        }
    }
}
