using System.Diagnostics.Contracts;
using OlnlineBanking.Infrastructure.Concrete;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Abstract
{
    [ContractClass(typeof(PassportContract))]
    public interface IPassport
    {
        LoginResult Login(UserLoginViewModel userLoginInfo);
        RegisterResult Register(UserRegisterViewModel userRegisterInfo);
        bool CheckUserExistence(string login, string email);
        User GetUserByLogin(string login);
        void ActivateUser(string login);
        bool BlockUser(string login, UserBlockAttemptCollection userBlockCountingCollection);
        void UnblockUser(string login);
        void Logout();
    }
}
