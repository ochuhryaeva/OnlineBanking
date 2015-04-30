using System.Collections.Generic;
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
                    UpdateUserEntry(userEntry,user);
                }
            }
            _context.SaveChanges();
        }

        private void UpdateUserEntry(User userTarget, User userSource)
        {
            foreach (var property in userSource.GetType().GetProperties())
            {
                var value = userSource.GetType().GetProperty(property.Name).GetValue(userSource);
                userSource.GetType().GetProperty(property.Name).SetValue(userSource, value);
            }
        }
    }
}
