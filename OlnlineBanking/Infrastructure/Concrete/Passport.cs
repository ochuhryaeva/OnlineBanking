using System;
using System.Linq;
using System.Reflection;
using System.Web.Security;
using log4net;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class Passport : IPassport
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfig _configManager;

        private readonly ILog _logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public Passport(IUserRepository userRepository, IConfig configManager)
        {
            _userRepository = userRepository;
            _configManager = configManager;
        }

        public LoginResult Login(UserLoginViewModel userLogin)
        {
            LoginResult loginResult = LoginResult.LrError;
            User user =
                _userRepository.Users.FirstOrDefault(
                    u => (u.Login == userLogin.Login) && (u.Password == userLogin.Password) &&
                         (u.IsActivated) && (!u.IsBlocked));
            if (user != null)
            {
                if (FormsAuthentication.IsEnabled) FormsAuthentication.SetAuthCookie(userLogin.Login, userLogin.RememberMe);
                loginResult = LoginResult.LrSuccess;
            }
            else
            {
                User userError = _userRepository.Users.FirstOrDefault(u => (u.Login == userLogin.Login));
                if (userError == null) loginResult = LoginResult.LrUserNotExist;
                else if (!userError.IsActivated) loginResult = LoginResult.LrUserNotActivated;
                else if (userError.IsBlocked) loginResult = LoginResult.LrUserIsBlocked;
                else if ((userError.Login == userLogin.Login) && (userError.Password != userLogin.Password))
                    loginResult = LoginResult.LrWrongPassword;
            }
            return loginResult;
        }

        public RegisterResult Register(UserRegisterViewModel userRegisterInfo)
        {
            RegisterResult registerResult = RegisterResult.RrError;
            //check if user with the same login or email already exists
            if (CheckUserExistence(userRegisterInfo.Login, userRegisterInfo.Email))
            {
                registerResult = RegisterResult.RrUserAlreadyExist;
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
                registerResult = RegisterResult.RrSuccess;
            }
            return registerResult;
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }


        public void ActivateUser(string login)
        {
            User user = _userRepository.Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                user.IsActivated = true;
                _userRepository.SaveUser(user);
            }
        }

        public void UnblockUser(string login)
        {
            User user = _userRepository.Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                user.IsBlocked = false;
                _userRepository.SaveUser(user);
                _logger.Info(String.Format("user with login: {0} was unblocked", login));
            }
        }

        public bool BlockUser(string login, UserBlockAttemptCollection userBlockAttemptCollection)
        {
            bool result = false;
            UserBlockAttempt userBlockAttempt = userBlockAttemptCollection.FirstOrDefault(u => (u.Login == login));
            if (userBlockAttempt == null) 
                userBlockAttemptCollection.Add(new UserBlockAttempt() {Login = login, LoginAttemptsCount = 1});
            else userBlockAttempt.LoginAttemptsCount++;
            if (userBlockAttemptCollection.FirstOrDefault(u=>(u.Login==login)&&(u.LoginAttemptsCount==_configManager.PassportSetting.BlockAttempts ))!=null)
            {
                User user = _userRepository.Users.FirstOrDefault(u => u.Login == login);
                if (user != null)
                {
                    user.IsBlocked = true;
                    _userRepository.SaveUser(user);
                    userBlockAttemptCollection.Remove(userBlockAttempt);
                    _logger.Info(String.Format("user with login: {0} was blocked", login));
                    result = true;
                }    
            }
            return result;
        }

        public bool CheckUserExistence(string login, string email)
        {
            return (_userRepository.Users.FirstOrDefault(u => (u.Login == login) || (u.Email == email)) != null);
        }



        public User GetUserByLogin(string login)
        {
            return _userRepository.Users.FirstOrDefault(u => u.Login == login);
        }
    }
}
