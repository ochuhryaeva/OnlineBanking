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
    public class UserRepository:IUserRepository
    {
        UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> Users
        {
            get
            {
                return _context.Users;

            } 
        }

        public void SaveUser(User user)
        {
            if (user.Id == 0)
            {
                _context.Users.Add(user);
            }
            else
            {
                User userEntry = _context.Users.Find(user.Id);
                if (userEntry != null)
                {
                    foreach (var property in user.GetType().GetProperties())
                    {
                        var value = user.GetType().GetProperty(property.Name).GetValue(user);
                        userEntry.GetType().GetProperty(property.Name).SetValue(userEntry, value);
                    }
                }
            }
            _context.SaveChanges();

        }
    }
}
