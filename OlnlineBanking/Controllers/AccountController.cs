using System;
using System.Web.Mvc;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Infrastructure.Concrete;
using OlnlineBanking.Models;

namespace OlnlineBanking.Controllers
{
    public class AccountController : Controller
    {
        private IPassport _passport;
        private IEmailService _emailService;

        public AccountController(IPassport passport, IEmailService emailService)
        {
            _passport = passport;
            _emailService = emailService;
        }
        public ActionResult Login()
        {
            return View(new UserLoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(UserLoginViewModel userLoginViewModel)
        {
            if (ModelState.IsValid)
            {
                LoginResult loginResult = _passport.Login(userLoginViewModel);
                if (loginResult==LoginResult.LrSuccess)
                {
                    return RedirectToAction("Index", "Client");
                }
                else
                {
                    RefreshErrors(loginResult);
                    if (loginResult == LoginResult.LrWrongPassword)
                    {
                        if (_passport.BlockUser(userLoginViewModel.Login, GetUserBlockAttemptCollection()))
                        {
                            User user = _passport.GetUserByLogin(userLoginViewModel.Login);
                            if (user != null) SendBlockEmail(user.Login, user.Email);
                            return RedirectToAction("UserIsBlocked");
                        }
                    }
                    return View(userLoginViewModel);
                }
            }
            else
            {
                return View(userLoginViewModel);
            }
        }

        public ActionResult Register()
        {
            return View(new UserRegisterViewModel());
        }

        [HttpPost]
        public ActionResult Register(UserRegisterViewModel userRegisterViewModel)
        {
            if (ModelState.IsValid)
            {
                RegisterResult registerResult = _passport.Register(userRegisterViewModel);
                if (registerResult==RegisterResult.RrSuccess)
                {
                    SendActivationEmail(userRegisterViewModel.Login,userRegisterViewModel.Email);
                    return RedirectToAction("NeedActivate");
                }
                else
                {
                    if (registerResult == RegisterResult.RrUserAlreadyExist)
                    {
                        ModelState.AddModelError("", "User with the same login or email is already exists");
                    }
                    return View(userRegisterViewModel);
                }
            }
            else
            {
                return View(userRegisterViewModel);
            }
        }

        [ChildActionOnly]
        public PartialViewResult UserInfo()
        {
            return PartialView();
        }

        public ActionResult Logout()
        {
            _passport.Logout();
            return RedirectToAction("Index", "Client");
        }

        public ActionResult ActivateUser(string userName)
        {
            _passport.ActivateUser(userName);
            string msg = "Your account is activated";
            return View("Message",(object)msg);    
        }

        public ActionResult UnblockUser(string userName)
        {
            _passport.UnblockUser(userName);
            string msg = "Your account is unblocked";
            return View("Message",(object)msg);
        }

        public ActionResult NeedActivate()
        {
            string msg = "The letter with activation was sent on your email";
            return View("Message", (object)msg);
        }

        public ActionResult UserIsBlocked()
        {
            string msg = "Your user is blocked. Message with unblock information was sent on your email";
            return View("Message", (object)msg);
        }

        private void RefreshErrors(LoginResult loginResult)
        {
            switch (loginResult)
            {
                case LoginResult.LrUserNotExist:
                case LoginResult.LrWrongPassword:
                    ModelState.AddModelError("","Login or password information isn't correct");
                    break;
                case LoginResult.LrUserNotActivated:
                    ModelState.AddModelError("","Your user is not activated");
                    break;
                case LoginResult.LrUserIsBlocked:
                    ModelState.AddModelError("","Your user is blocked");
                    break;
                case LoginResult.LrError:
                    ModelState.AddModelError("", "Error during login");
                    break;
                default:
                    break;
            }
        }
        
        private void SendActivationEmail(string login, string email)
        {
            string subject = "Register confirmation";
            string confirmationLink = String.Empty;
            if (Url != null)
            {
                confirmationLink = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ActivateUser", "Account", new { userName = login });
            }
            string body = "Enter the link for account activation: <a href=\"" + confirmationLink +"\">activate account</a>";
            _emailService.SendEmail(email,subject,body);
        }

        private void SendBlockEmail(string login, string email)
        {
            string subject = "Block information";
            string blockLink = String.Empty;
            if (Url != null)
            {
                blockLink = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("UnblockUser", "Account", new { userName = login });
            }
            string body = "Enter the link to unblock your account: <a href=\"" + blockLink + "\">unblock account</a>";
            _emailService.SendEmail(email, subject, body);
        }

        private UserBlockAttemptCollection GetUserBlockAttemptCollection()
        {
            
            UserBlockAttemptCollection userBlockAttemptCollection = null;
            if (ControllerContext!=null && ControllerContext.HttpContext.Session != null)
            {
                userBlockAttemptCollection = (UserBlockAttemptCollection)ControllerContext.HttpContext.Session["UserBlockAttemptCollection"];
            }
            // create the userBlockAtemptCollection if there wasn't one in the session data
            if (userBlockAttemptCollection == null)
            {
                userBlockAttemptCollection = new UserBlockAttemptCollection();
                if (ControllerContext != null && ControllerContext.HttpContext.Session != null)
                {
                    ControllerContext.HttpContext.Session["UserBlockAttemptCollection"] = userBlockAttemptCollection;
                }
            }
            return userBlockAttemptCollection;
        }

    }
}