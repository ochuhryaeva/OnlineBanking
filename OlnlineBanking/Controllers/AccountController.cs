using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
                    if (loginResult==LoginResult.LrWrongPassword) RefreshBlockStatus(userLoginViewModel.Login);
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
                default:
                    break;
            }
        }

        private void RefreshBlockStatus(string login)
        {
            if (ControllerContext.HttpContext.Session != null)
            {
                Dictionary<string, int> blockStatus = (Dictionary<string, int>)ControllerContext.HttpContext.Session["BlockStatus"];
                blockStatus = blockStatus?? new Dictionary<string, int>();
                if (!blockStatus.ContainsKey(login))
                {
                    blockStatus[login] = 1;
                }
                else
                {
                    blockStatus[login]++;
                }
                //TODO: Change on 5
                //block user
                if (blockStatus[login] == 3)
                {
                    _passport.BlockUser(login);
                    blockStatus.Remove(login);
                    User user = _passport.GetUserByLogin(login);
                    if(user!=null) SendBlockEmail(user.Login,user.Email);
                    ModelState.AddModelError("","Your user is blocked. Message with unblock information was sent on your email");
                }
                ControllerContext.HttpContext.Session["BlockStatus"] = blockStatus;
            }
        }

        private void SendActivationEmail(string login, string email)
        {
            string subject = "Register confirmation";
            //TODO: what to do with the port
            string confirmationLink = String.Empty;
            if(Url!=null) confirmationLink = "http://localhost:50387" + Url.Action("ActivateUser", "Account", new { userName = login });
            string body = "Enter the link for account activation: <a href=\"" + confirmationLink +"\">activate account</a>";
            _emailService.SendEmail(email,subject,body);
        }

        private void SendBlockEmail(string login, string email)
        {
            string subject = "Block information";
            string blockLink = "http://localhost:50387" + Url.Action("UnblockUser", "Account", new { userName = login });
            string body = "Enter the link to unblock your account: <a href=\"" + blockLink + "\">unblock account</a>";
            _emailService.SendEmail(email, subject, body);
        }
        
    }
}