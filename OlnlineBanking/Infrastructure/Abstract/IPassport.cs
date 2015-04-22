﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Microsoft.Owin.Security;
using OlnlineBanking.Infrastructure.Concrete;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Abstract
{
    [ContractClass(typeof(PassportContract))]
    public interface IPassport
    {
        LoginResult Login(UserLoginViewModel userLoginInfo);
        RegisterResult Register(UserRegisterViewModel userRegisterInfo);
        User GetUserByLogin(string login);
        void ActivateUser(string userName);
        void BlockUser(string userName);
        void UnblockUser(string userName);
        void Logout();
    }
}
