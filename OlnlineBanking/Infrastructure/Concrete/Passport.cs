using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class Passport: IPassport
    {

        //TODO: add IUserRepository to AutoFac
        readonly IUserRepository _userRepository = new UserRepository();
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public LoginResult Login(UserLoginViewModel userLogin)
        {
            LoginResult loginResult = LoginResult.LrError;
            User user = _userRepository.Users.FirstOrDefault(u => (u.Login == userLogin.Login) && (u.Password == userLogin.Password)&&
                (u.IsActivated)&&(!u.IsBlocked));
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(userLogin.Login, userLogin.RememberMe);
                loginResult = LoginResult.LrSuccess;
            } 
            else
            {
                User userError = _userRepository.Users.FirstOrDefault(u => (u.Login == userLogin.Login));
                if (userError==null) loginResult=LoginResult.LrUserNotExist;
                else if (!userError.IsActivated) loginResult=LoginResult.LrUserNotActivated;
                else if (userError.IsBlocked) loginResult=LoginResult.LrUserIsBlocked;
                else if ((userError.Login==userLogin.Login)&&(userError.Password!=userLogin.Password)) loginResult=LoginResult.LrWrongPassword;
            }
            return loginResult;
        }

        public RegisterResult Register(UserRegisterViewModel userRegisterInfo)
        {
            RegisterResult registerResult = RegisterResult.RrError;
            //check if user with the same login or email already exists
            if (CheckUserExistence(userRegisterInfo.Login, userRegisterInfo.Email))
            {
                registerResult=RegisterResult.RrUserAlreadyExist;
            }
            //add new user
            else
            {
                User user = new User()
                {
                    Login = userRegisterInfo.Login,
                    Email = userRegisterInfo.Email,
                    Address = userRegisterInfo.Address,
                    Password = userRegisterInfo.Password
                };
                _userRepository.SaveUser(user);
                registerResult=RegisterResult.RrSuccess;
            }
            return registerResult;
        }
        
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }


        public void ActivateUser(string userName)
        {
            User user = _userRepository.Users.FirstOrDefault(u=>u.Login==userName);
            if (user != null)
            {
                user.IsActivated = true;
                _userRepository.SaveUser(user);
            }
        }

        public void UnblockUser(string userName)
        {
            User user = _userRepository.Users.FirstOrDefault(u => u.Login == userName);
            if (user != null)
            {
                user.IsBlocked = false;
                _userRepository.SaveUser(user);
                _logger.Info(String.Format("user with login: {0} was unblocked", userName));
            }
        }

        public void BlockUser(string userName)
        {
            User user = _userRepository.Users.FirstOrDefault(u => u.Login == userName);
            if (user != null)
            {
                user.IsBlocked = true;
                _userRepository.SaveUser(user);
                _logger.Info(String.Format("user with login: {0} was blocked",userName));
            }
        }

        private bool CheckUserExistence(string login, string email)
        {
            return (_userRepository.Users.FirstOrDefault(u => (u.Login == login) || (u.Email == email)) != null);
        }



        public User GetUserByLogin(string login)
        {
            return _userRepository.Users.FirstOrDefault(u => u.Login == login);
        }
    }
}
