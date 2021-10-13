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
using System.Globalization;

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
                if(principal.Login.Suspend && !principal.Admin)
                {
                    ViewBag.result = "Your account is Suspend Please Contact Admin";
                    return View();
                }
                if(!principal.Login.Verify && !principal.Admin)
                {
                    ViewBag.result = "Your account is Unverify Please Contact Admin";
                    return View();
                }
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
        public async Task<IActionResult> register()
        {
            ViewBag.user = await _iuserService.GetusernameEmailList();
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
                    brithday = DateTime.Parse(HttpContext.Request.Form["BR"].ToString(),new CultureInfo("en-GB"));
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
        [HttpGet("/user/Getunverify")]
        public async Task<IActionResult> GetUnverify()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long user);
            User principal = _iuserService.GetWithoutPassword(user);
            if(principal.Admin)
            {
                List<User> UnverifyUsers = await _iuserService.GetUnverifyAsync();
                ViewBag.data = UnverifyUsers;
                return View();
            }
            return Redirect("/Home/Restricted");
        }
        [HttpPost("/user/Getunverify")]
        public IActionResult SetUnverify()
        {
            try
            {
                long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
                long.TryParse(Request.Form["id"].ToString(), out long Verifyuser);
                User principal = _iuserService.GetWithoutPassword(Adminuser);
                if (principal.Admin)
                {
                    var res = _iuserService.SetVerifyByadmin(Verifyuser);
                    if (res==1)
                    {
                        ViewBag.res = "Success";
                    }
                    else if(res==0)
                    {
                        ViewBag.res = "Unsuccess";
                    }
                    if(res==2)
                    {
                        return Redirect("/user/Getunverify");
                    }
                    List<User> UnverifyUsers = _iuserService.GetUnverifyAsync().GetAwaiter().GetResult();
                    ViewBag.data = UnverifyUsers;
                    return View("Getunverify");
                }
                return Redirect("/Home/Restricted");
            }
            catch(Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }
        }

    }
}
