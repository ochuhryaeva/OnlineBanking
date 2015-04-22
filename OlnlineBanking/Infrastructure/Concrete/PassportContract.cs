using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    [ContractClassFor(typeof(IPassport))]
    public abstract class PassportContract: IPassport
    {
        public LoginResult Login(Models.UserLoginViewModel userLoginInfo)
        {
            Contract.Requires(userLoginInfo!=null);
            return 0;
        }

        public RegisterResult Register(Models.UserRegisterViewModel userRegisterInfo)
        {
            Contract.Requires(userRegisterInfo != null);
            return 0;
        }

        public User GetUserByLogin(string login)
        {
            Contract.Requires(!string.IsNullOrEmpty(login));
            return null;
        }

        public void ActivateUser(string userName)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName));
        }

        public void BlockUser(string userName)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName));
        }

        public void UnblockUser(string userName)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName));
        }

        public void Logout()
        {
            //    
        }
    }
}