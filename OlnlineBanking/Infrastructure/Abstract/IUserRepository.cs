using System.Collections.Generic;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Abstract
{
    public interface IUserRepository
    {
        IEnumerable<User> Users { get; }
        void SaveUser(User user);
    }
}
