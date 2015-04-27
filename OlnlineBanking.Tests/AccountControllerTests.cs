using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using OlnlineBanking.Controllers;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Infrastructure.Concrete;
using OlnlineBanking.Models;

namespace OlnlineBanking.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {

        private Mock<IPassport> _passportMock;
        private Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void SetUp()
        {
            _passportMock = new Mock<IPassport>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        private AccountController InitAccountController()
        {
            AccountController controller = new AccountController(_passportMock.Object, _emailServiceMock.Object);
            return controller;
        }

        private UserLoginViewModel GetUserLoginViewModel()
        {
            return new UserLoginViewModel()
            {
                Login = "Olga",
                Password = "Olga"
            };
        }

        private UserRegisterViewModel GetUserRegisterModel()
        {
            return new UserRegisterViewModel()
            {
                Login = "Olga",
                Password = "Olga",
                ConfirmPassword = "Olga",
                Email = "ochuhryaeva@gmail.com",
                Id = 1
            };
        }

        [Test]
        public void Login_ShowLoginView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            //Act
            ActionResult result = controller.Login();
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserLoginViewModel>(((ViewResult)result).Model);
        }

        [Test]
        public void Login_WithValidModel_CallLoginAndRedirect()
        {
            // Arrange - controller
            AccountController controller = InitAccountController(); UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            //Act
            ActionResult result = controller.Login(userLoginViewModel);
            //Assert
            _passportMock.Verify(m => m.Login(userLoginViewModel));
            // Assert - check the method result type
            Assert.IsNotInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Login_WithInvalidModel_AddErrorNotLoginShowErrorView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            //Act
            ActionResult result = controller.Login(userLoginViewModel);
            //Assert
            _passportMock.Verify(m => m.Login(It.IsAny<UserLoginViewModel>()),Times.Never);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        
        [Test]
        public void Login_IfLoginResultIsUserNotExist_ShowErrorView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            _passportMock.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrUserNotExist);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Login_IfLoginResultIsUserNotActivated_ShowErrorView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            _passportMock.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrUserNotActivated);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Login_IfLoginResultIsUserIsBlocked_ShowErrorView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            _passportMock.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrUserIsBlocked);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Login_IfLoginResultIsError_ShowErrorView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            _passportMock.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrError);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Login_IfLoginResultIsSuccess_MakeRedirect()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            _passportMock.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrSuccess);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsNotInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, true);
        }

        [Test]
        public void Login_IfUserWasBlocked_SendEmail()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            _passportMock.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrWrongPassword);
            _passportMock.Setup(m => m.BlockUser(userLoginViewModel.Login,It.IsAny<UserBlockAttemptCollection>())).Returns(true);
            string email = "1@1.com";
            _passportMock.Setup(m => m.GetUserByLogin(userLoginViewModel.Login))
                .Returns(new User() {Login = userLoginViewModel.Login, Email = email});
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            _emailServiceMock.Verify(e => e.SendEmail(email,It.IsAny<string>(),It.IsAny<string>()));
        }

        [Test]
        public void Register_ShowRegisterView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            //Act
            ActionResult result = controller.Register();
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserRegisterViewModel>(((ViewResult)result).Model);
        }

        [Test]
        public void Register_WithValidModel_CallRegister()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            _emailServiceMock.Setup(s => s.SendEmail(userRegisterViewModel.Email,"subject","body"));
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            _passportMock.Verify(m => m.Register(userRegisterViewModel));
            // Assert - check the method result type
            Assert.IsNotInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Register_WithInvalidModel_NotCallRegisterAndShowError()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            controller.ModelState.AddModelError("error", "error");
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            _passportMock.Verify(m => m.Register(It.IsAny<UserRegisterViewModel>()), Times.Never);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Register_IfUserAlreadyExists_ShowErrorView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            _passportMock.Setup(m => m.Register(userRegisterViewModel)).Returns(RegisterResult.RrUserAlreadyExist);
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(false,controller.ModelState.IsValid);
        }

        [Test]
        public void Register_ToActivateValidUser_SendEmail()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            _passportMock.Setup(m => m.Register(userRegisterViewModel)).Returns(RegisterResult.RrSuccess);
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            _emailServiceMock.Verify(e => e.SendEmail(userRegisterViewModel.Email, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void Register_AfterRegistration_ShowNeedActivateMessage()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            _passportMock.Setup(m => m.Register(userRegisterViewModel)).Returns(RegisterResult.RrSuccess);
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            Assert.IsNotInstanceOf<ViewResult>(result);
            string actionName = (((RedirectToRouteResult) result).RouteValues["action"] as string).ToUpper();
            Assert.AreEqual(actionName,"NEEDACTIVATE");
            //((RedirectToRouteResult)result)
         }

        [Test]
        public void Logout_CallLogOut()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            // Act
            controller.Logout();
            //Assert
            _passportMock.Verify(p=>p.Logout());
        }

        [Test]
        public void ActivateUser_CallActivateUser()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            string userName = "Olga";
            // Act
            controller.ActivateUser(userName);
            //Assert
            _passportMock.Verify(p => p.ActivateUser(userName));
        }

        [Test]
        public void UnblockUser_CallUnblockUser()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            string userName = "Olga";
            // Act
            controller.UnblockUser(userName);
            //Assert
            _passportMock.Verify(p => p.UnblockUser(userName));
        }

        [Test]
        public void UserInfo_ShowUserInfoAsPartialView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            // Act
            ActionResult result = controller.UserInfo();
            // Assert
            Assert.IsInstanceOf<PartialViewResult>(result);
        }

        [Test]
        public void NeedActivate_ShowViewMessage()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            // Act
            ActionResult result = controller.NeedActivate();
            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("message",((ViewResult)result).ViewName.ToLower());
        }

        [Test]
        public void UserIsBlocked_ShowViewMessage()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            // Act
            ActionResult result = controller.UserIsBlocked();
            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("message", ((ViewResult)result).ViewName.ToLower());
        }

        [Test]
        public void ActivateUser_ShowViewMessage()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            // Act
            ActionResult result = controller.ActivateUser("user");
            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("message", ((ViewResult)result).ViewName.ToLower());
        }

        [Test]
        public void UnblockUser_ShowViewMessage()
        {
            // Arrange - controller
            AccountController controller = InitAccountController();
            // Act
            ActionResult result = controller.UnblockUser("user");
            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("message", ((ViewResult)result).ViewName.ToLower());
        }
        
    }
}
