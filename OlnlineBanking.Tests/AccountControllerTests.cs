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
        private Mock<IPassport> GetIPassportMock()
        {
            Mock<IPassport> mock = new Mock<IPassport>();
            return mock;
        }

        private Mock<IEmailService> GetIEmailServiceMock()
        {
            Mock<IEmailService> mock = new Mock<IEmailService>();
            return mock;
        }

        private AccountController InitAccountController(Mock<IPassport> mockIPassport, Mock<IEmailService> mockIEmailService)
        {
            AccountController controller = new AccountController(mockIPassport.Object, mockIEmailService.Object);
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
        public void Can_ShowLoginView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController(GetIPassportMock(),GetIEmailServiceMock());
            //Act
            ActionResult result = controller.Login();
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserLoginViewModel>(((ViewResult)result).Model);
        }

        [Test]
        public void Can_LoginWithValidModel()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport,mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            //Act
            ActionResult result = controller.Login(userLoginViewModel);
            //Assert
            mockIPassport.Verify(m => m.Login(userLoginViewModel));
            // Assert - check the method result type
            Assert.IsNotInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Can_LoginWithInvalidModel()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            //Act
            ActionResult result = controller.Login(userLoginViewModel);
            //Assert
            mockIPassport.Verify(m => m.Login(It.IsAny<UserLoginViewModel>()),Times.Never);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        
        [Test]
        public void Can_ShowErrorViewIfLoginResultIsUserNotExist()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            mockIPassport.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrUserNotExist);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Can_ShowErrorViewIfLoginResultIsUserNotActivated()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            mockIPassport.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrUserNotActivated);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Can_ShowErrorViewIfLoginResultIsUserIsBlocked()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            mockIPassport.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrUserIsBlocked);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Can_ShowErrorViewIfLoginResultIsError()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            mockIPassport.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrError);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
        }

        [Test]
        public void Can_MakeRedirectIfLoginResultIsSuccess()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            mockIPassport.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrSuccess);
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsNotInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, true);
        }

        [Test]
        public void Can_SendEmailIfUserWasBlocked()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserLoginViewModel userLoginViewModel = GetUserLoginViewModel();
            mockIPassport.Setup(m => m.Login(userLoginViewModel)).Returns(LoginResult.LrWrongPassword);
            mockIPassport.Setup(m => m.BlockUser(userLoginViewModel.Login,It.IsAny<UserBlockAttemptCollection>())).Returns(true);
            string email = "1@1.com";
            mockIPassport.Setup(m => m.GetUserByLogin(userLoginViewModel.Login))
                .Returns(new User() {Login = userLoginViewModel.Login, Email = email});
            //act
            ActionResult result = controller.Login(userLoginViewModel);
            //
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(controller.ModelState.IsValid, false);
            mockIEmailService.Verify(e => e.SendEmail(email,It.IsAny<string>(),It.IsAny<string>()));
        }

        [Test]
        public void Can_ShowRegisterView()
        {
            // Arrange - controller
            AccountController controller = InitAccountController(GetIPassportMock(), GetIEmailServiceMock());
            //Act
            ActionResult result = controller.Register();
            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserRegisterViewModel>(((ViewResult)result).Model);
        }

        [Test]
        public void Can_RegisterWithValidModel()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            mockIEmailService.Setup(s => s.SendEmail(userRegisterViewModel.Email,"subject","body"));
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            mockIPassport.Verify(m => m.Register(userRegisterViewModel));
            // Assert - check the method result type
            Assert.IsNotInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Can_RegisterWithInvalidModel()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            controller.ModelState.AddModelError("error", "error");
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            mockIPassport.Verify(m => m.Register(It.IsAny<UserRegisterViewModel>()), Times.Never);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Can_SendEmailToActivateValidUser()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            mockIPassport.Setup(m => m.Register(userRegisterViewModel)).Returns(RegisterResult.RrSuccess);
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            mockIEmailService.Verify(e => e.SendEmail(userRegisterViewModel.Email, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void Can_ShowNeedActivateMessageAfterRegistration()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            UserRegisterViewModel userRegisterViewModel = GetUserRegisterModel();
            mockIPassport.Setup(m => m.Register(userRegisterViewModel)).Returns(RegisterResult.RrSuccess);
            //Act
            ActionResult result = controller.Register(userRegisterViewModel);
            //Assert
            Assert.IsNotInstanceOf<ViewResult>(result);
            string actionName = (((RedirectToRouteResult) result).RouteValues["action"] as string).ToUpper();
            Assert.AreEqual(actionName,"NEEDACTIVATE");
            //((RedirectToRouteResult)result)
         }

        [Test]
        public void Can_LogOut()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            // Act
            controller.Logout();
            //Assert
            mockIPassport.Verify(p=>p.Logout());
        }

        [Test]
        public void Can_ActivateUser()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            string userName = "Olga";
            // Act
            controller.ActivateUser(userName);
            //Assert
            mockIPassport.Verify(p => p.ActivateUser(userName));
        }

        [Test]
        public void Can_UnblockUser()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            string userName = "Olga";
            // Act
            controller.UnblockUser(userName);
            //Assert
            mockIPassport.Verify(p => p.UnblockUser(userName));
        }

        [Test]
        public void Can_ShowUserInfoAsPartialView()
        {
            // Arrange - controller
            Mock<IPassport> mockIPassport = GetIPassportMock();
            Mock<IEmailService> mockIEmailService = GetIEmailServiceMock();
            AccountController controller = InitAccountController(mockIPassport, mockIEmailService);
            // Act
            ActionResult result = controller.UserInfo();
            // Assert
            Assert.IsInstanceOf<PartialViewResult>(result);
        }

        
    }
}
