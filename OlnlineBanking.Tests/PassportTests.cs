using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Infrastructure.Concrete;
using OlnlineBanking.Infrastructure.Config;
using OlnlineBanking.Models;

namespace OlnlineBanking.Tests
{
    [TestFixture]
    public class PassportTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IConfig> _configMock;

        [SetUp]
        public void SetUp()
        {
           _userRepositoryMock = new Mock<IUserRepository>();
            _userRepositoryMock.Setup(u => u.Users).Returns(GetUsers());
           _configMock = new Mock<IConfig>();
        }

        private List<User> GetUsers()
        {
            List<User> users = new List<User>()
            {
                //Correct User
                new User()
                {
                    Id = 1,
                    Login = "login1",
                    Email = "login1@host.com",
                    IsActivated = true,
                    IsBlocked = false,
                    Password = "login1"
                },
                //Not activated user
                new User()
                {
                    Id = 2,
                    Login = "login2",
                    Email = "login2@host.com",
                    IsActivated = false,
                    IsBlocked = false,
                    Password = "login2"
                },
                //Blocked user
                new User()
                {
                    Id = 3,
                    Login = "login3",
                    Email = "login3@host.com",
                    IsActivated = true,
                    IsBlocked = true,
                    Password = "login2"
                },
            };
            return users;
        }

        private Passport GetPassport()
        {
            return new Passport(_userRepositoryMock.Object,_configMock.Object);
        }
        
        [Test]
        public void Login_ForCorrectUser_ReturnSuccess()
        {
            //Arrange
            Passport passport = GetPassport();
            UserLoginViewModel userLoginViewModel = new UserLoginViewModel()
            {
                Login = "login1",
                Password = "login1"
            };
            //Act
            LoginResult result = passport.Login(userLoginViewModel);
            //Assert
            Assert.AreEqual(LoginResult.LrSuccess,result);
        }

        [Test]
        public void Login_ForNotExistingUser_ReturnUserNotExist()
        {
            //Arrange
            Passport passport = GetPassport();
            UserLoginViewModel userLoginViewModel = new UserLoginViewModel()
            {
                Login = "login8",
                Password = "login8"
            };
            //Act
            LoginResult result = passport.Login(userLoginViewModel);
            //Assert
            Assert.AreEqual(LoginResult.LrUserNotExist, result);
        }

        [Test]
        public void Login_InputWrongPassword_ReturnWrongPassword()
        {
            //Arrange
            Passport passport = GetPassport();
            UserLoginViewModel userLoginViewModel = new UserLoginViewModel()
            {
                Login = "login1",
                Password = "wrongpassword"
            };
            //Act
            LoginResult result = passport.Login(userLoginViewModel);
            //Assert
            Assert.AreEqual(LoginResult.LrWrongPassword, result);
        }

        [Test]
        public void Login_UserIsNotActivated_ReturnNotActivated()
        {
            //Arrange
            Passport passport = GetPassport();
            UserLoginViewModel userLoginViewModel = new UserLoginViewModel()
            {
                Login = "login2",
                Password = "login2"
            };
            //Act
            LoginResult result = passport.Login(userLoginViewModel);
            //Assert
            Assert.AreEqual(LoginResult.LrUserNotActivated, result);
        }

        [Test]
        public void Login_IfUserWasBlocked_ReturnUserIsBlocked()
        {
            Passport passport = GetPassport();
            UserLoginViewModel userLoginViewModel = new UserLoginViewModel()
            {
                Login = "login3",
                Password = "login3"
            };
            //Act
            LoginResult result = passport.Login(userLoginViewModel);
            //Assert
            Assert.AreEqual(LoginResult.LrUserIsBlocked, result);
        }

        [Test]
        public void Register_ForCorrectUser_AddUserAndReturnSuccess()
        {
            Passport passport = GetPassport();
            UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel()
            {
                Id = 0,
                Login = "login10",
                Password = "login10",
                ConfirmPassword = "login10",
                Email = "login10@host.com",
                Address = ""
            };
            User user = new User()
            {
                Login = userRegisterViewModel.Login,
                Email = userRegisterViewModel.Email,
                Address = userRegisterViewModel.Address,
                Password = userRegisterViewModel.Password
            };
            //Act
            RegisterResult result = passport.Register(userRegisterViewModel);
            //Assert
            Assert.AreEqual(RegisterResult.RrSuccess, result);
            _userRepositoryMock.Verify(u => u.SaveUser(It.Is<User>(us => (us.Login == user.Login)
                                                                         && (us.Password == user.Password)
                                                                         && (us.Email == user.Email)
                                                                         && (us.Address == user.Address)
                                                                         && (us.IsActivated == false)
                                                                         && (us.IsBlocked == false)
                                                                         && (us.Id == 0))));
           
        }

        [Test]
        public void Register_ForExistedUser_ReturnUserAlreadyExistAndNotAddUser()
        {
            Passport passport = GetPassport();
            UserRegisterViewModel userRegisterViewModel = new UserRegisterViewModel()
            {
                Id = 0,
                Login = "login1",
                Password = "login1",
                ConfirmPassword = "login1",
                Email = "login1@host.com",
                Address = ""
            };
            User user = new User()
            {
                Login = userRegisterViewModel.Login,
                Email = userRegisterViewModel.Email,
                Address = userRegisterViewModel.Address,
                Password = userRegisterViewModel.Password
            };
            //Act
            RegisterResult result = passport.Register(userRegisterViewModel);
            //Assert
            Assert.AreEqual(RegisterResult.RrUserAlreadyExist, result);
            _userRepositoryMock.Verify(u => u.SaveUser(It.IsAny<User>()),Times.Never());
        }

        [Test]
        public void ActivateUser_ChangeIsActivatedToTrueAndSaveUser()
        {
            //Arrange
            Passport passport = GetPassport();
            string userLogin = "login2";
            User user = _userRepositoryMock.Object.Users.FirstOrDefault(u => u.Login == userLogin);
            //Act
            passport.ActivateUser(userLogin);
            //Assert
            _userRepositoryMock.Verify(u => u.SaveUser(It.Is<User>(us => (us.Login == user.Login)
                                                                         && (us.Password == user.Password)
                                                                         && (us.Email == user.Email)
                                                                         && (us.Address == user.Address)
                                                                         && (us.IsActivated == true)
                                                                         && (us.IsBlocked == user.IsBlocked)
                                                                         && (us.Id == user.Id))));
        }

        [Test]
        public void UnblockUser_ChangeIsBlockedToFalseAndSaveUser()
        {
            //Arrange
            Passport passport = GetPassport();
            string userLogin = "login3";
            User user = _userRepositoryMock.Object.Users.FirstOrDefault(u => u.Login == userLogin);
            //Act
            passport.UnblockUser(userLogin);
            //Assert
            _userRepositoryMock.Verify(u => u.SaveUser(It.Is<User>(us => (us.Login == user.Login)
                                                                         && (us.Password == user.Password)
                                                                         && (us.Email == user.Email)
                                                                         && (us.Address == user.Address)
                                                                         && (us.IsActivated == user.IsActivated)
                                                                         && (us.IsBlocked == false)
                                                                         && (us.Id == user.Id))));
        }

        [Test]
        public void CheckUserExistence_ByLogin_ReturnTrueIfUserExists()
        {
            //Arrange
            Passport passport = GetPassport();
            string userLogin = "login1";
            //Act
            bool result = passport.CheckUserExistence(userLogin,"fff");
            //Assert
            Assert.AreEqual(true,result);
        }

        [Test]
        public void CheckUserExistence_ByEmail_ReturnTrueIfUserExists()
        {
            //Arrange
            Passport passport = GetPassport();
            string userEmail = "login1@host.com";
            //Act
            bool result = passport.CheckUserExistence("kuku", userEmail);
            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void CheckUserExistence_ByLogin_ReturnFalseIfUserNotExists()
        {
            //Arrange
            Passport passport = GetPassport();
            string userLogin = "notexist";
            //Act
            bool result = passport.CheckUserExistence(userLogin, "notexist@host.com");
            //Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void CheckUserExistence_ByEmail_ReturnFalseIfUserNotExists()
        {
            //Arrange
            Passport passport = GetPassport();
            string userEmail = "notexist@host.com";
            //Act
            bool result = passport.CheckUserExistence("kuku", userEmail);
            //Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void BlockUser_IfBlockAttemptsIsDone_SetIsBlockedToTrueAndSaveUser()
        {
            //Arrange
            Passport passport = GetPassport();
            PassportSetting passportSetting = new PassportSetting(){BlockAttempts = 3};
            _configMock.Setup(c => c.PassportSetting).Returns(passportSetting);
            string userLogin = "login1";
            User user = _userRepositoryMock.Object.Users.FirstOrDefault(u => u.Login == userLogin);
            UserBlockAttemptCollection userAttempts = new UserBlockAttemptCollection()
            {
                new UserBlockAttempt() {Login = userLogin, LoginAttemptsCount = _configMock.Object.PassportSetting.BlockAttempts-1}
            };
            //Act
            bool result = passport.BlockUser(userLogin, userAttempts);
            //Assert
            Assert.AreEqual(true,result);
            _userRepositoryMock.Verify(u => u.SaveUser(It.Is<User>(us => (us.Login == user.Login)
                                                                         && (us.Password == user.Password)
                                                                         && (us.Email == user.Email)
                                                                         && (us.Address == user.Address)
                                                                         && (us.IsActivated == user.IsActivated)
                                                                         && (us.IsBlocked == true)
                                                                         && (us.Id == user.Id))));
        }

        [Test]
        public void BlockUser_IfBlockAttemptsIsNotDone_NotChangeIsBlockedAndNotSaveUser()
        {
            //Arrange
            Passport passport = GetPassport();
            PassportSetting passportSetting = new PassportSetting() { BlockAttempts = 3 };
            _configMock.Setup(c => c.PassportSetting).Returns(passportSetting);
            string userLogin = "login1";
            User user = _userRepositoryMock.Object.Users.FirstOrDefault(u => u.Login == userLogin);
            UserBlockAttemptCollection userAttempts = new UserBlockAttemptCollection()
            {
                new UserBlockAttempt() {Login = userLogin, LoginAttemptsCount = _configMock.Object.PassportSetting.BlockAttempts-2}
            };
            //Act
            bool result = passport.BlockUser(userLogin, userAttempts);
            //Assert
            Assert.AreEqual(false, result);
            _userRepositoryMock.Verify(u => u.SaveUser(It.IsAny<User>()),Times.Never);
        }

        [Test]
        public void BlockUser_RefreshBlockAttemptsForUser()
        {
            //Arrange
            Passport passport = GetPassport();
            PassportSetting passportSetting = new PassportSetting() { BlockAttempts = 3 };
            _configMock.Setup(c => c.PassportSetting).Returns(passportSetting);
            string userLogin = "login1";
            User user = _userRepositoryMock.Object.Users.FirstOrDefault(u => u.Login == userLogin);
            UserBlockAttemptCollection userAttempts = new UserBlockAttemptCollection()
            {
                new UserBlockAttempt() {Login = userLogin, LoginAttemptsCount = _configMock.Object.PassportSetting.BlockAttempts-2}
            };
            //Act
            bool result = passport.BlockUser(userLogin, userAttempts);
            //Assert
            Assert.AreEqual(_configMock.Object.PassportSetting.BlockAttempts - 1,userAttempts.FirstOrDefault(u=>u.Login==userLogin).LoginAttemptsCount);

        }

        [Test]
        public void GetUserByLogin_ForExistLogin_ReturnCorrectUser()
        {
            //Arrange
            Passport passport = GetPassport();
            string userLogin = "login1";
            //Act
            User result = passport.GetUserByLogin(userLogin);
            //Assert
            Assert.AreEqual(userLogin,result.Login);
        }

        [Test]
        public void GetUserByLogin_ForNotExistLogin_ReturnNull()
        {
            //Arrange
            Passport passport = GetPassport();
            string userLogin = "login10";
            //Act
            User result = passport.GetUserByLogin(userLogin);
            //Assert
            Assert.AreEqual(null, result);
        }

    }
}
