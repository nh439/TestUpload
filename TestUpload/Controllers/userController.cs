using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Profile;
using TestUpload.Service;




namespace TestUpload.Controllers
{
    public class userController : Controller
    {
        private readonly IuserService _iuserService ;
        private readonly ILoginService _loginService;
        private readonly IhistoryLogService service;
        private ILogger<userController> _logger;
        public userController(IuserService iuserService, ILoginService loginService, IhistoryLogService ihistory, ILogger<userController> logger)
        {
            _iuserService = iuserService;
            _loginService = loginService;
            _logger = logger;
            service = ihistory;
        }

        [HttpGet("/user")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("user/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("user/login")]
        public IActionResult Logined()
        {
            string username = Request.Form["login"].ToString();
            string password = Request.Form["pass"].ToString();
            User principal = _loginService.GetLogin(username, password);
            if(principal != null)
            {
                // System.Web.HttpContext.Current.Session("userId") = principal.Id;
                HttpContext.Session.SetString("uid", principal.Id.ToString());
                HttpContext.Session.SetString("fn", principal.Firstname);
                HttpContext.Session.SetString("ln", principal.Lastname);
                HttpContext.Session.SetString("un", username);
                if (principal.Admin)
                {
                    HttpContext.Session.SetString("admin","1");
                }
                ViewBag.result = string.Empty;
                return Redirect("/");
            }
            ViewBag.result = "Username or Password Incorrect \n Please Try Again";
            return View();
        }
        [HttpGet("user/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
        [HttpGet("user/register")]
        public IActionResult register()
        {
            return View();
        }
        [HttpPost("user/register")]
        public IActionResult Registerd()
        {
            User principal = new User();
            string password, retype;
            password = HttpContext.Request.Form["pass"].ToString();
            retype = HttpContext.Request.Form["ret"].ToString();
            if(password==retype)
            {
                try
                {
                    string fn, ln, Email, Rules, username;
                    DateTime brithday;
                    bool male;
                    fn = HttpContext.Request.Form["Fn"].ToString();
                    ln = HttpContext.Request.Form["Ln"].ToString();
                    Email = HttpContext.Request.Form["Email"].ToString();
                    username = HttpContext.Request.Form["username"].ToString();
                    male = bool.Parse(HttpContext.Request.Form["Gender"].ToString());
                    brithday = DateTime.Parse(HttpContext.Request.Form["BR"].ToString());
                    principal = new User
                    {       
                        BrithDay = brithday,
                        Email = Email,
                        Firstname = fn,
                        Lastname = ln,
                        Login = new Login
                        {
                            Password = password,
                            Username = username
                        },
                        Male = male
                    };
                    var t = _iuserService.Register(principal);
                    if (t)
                    {
                        service.CreateSuccessHistory("User", "Register", null, 0);
                        ViewBag.result = "Registerd Successful";
                        return View();
                    }
                    ViewBag.result = "Registerd UnSuccessful";
                    return View();
                }
                catch(Exception x)
                {
                    _logger.LogError(x.Message);
                    ViewBag.result = "Registerd Corruped";
                    return View();
                }

            }
            ViewBag.result = "Password Retype Incorrect";
            return View();
        }
    }
}
