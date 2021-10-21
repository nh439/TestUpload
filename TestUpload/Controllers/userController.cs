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
using TestUpload.Securities;
using TestUpload.Models.criteria;

namespace TestUpload.Controllers
{
    public class userController : Controller
    {
        private readonly IuserService _iuserService;
        private readonly ILoginService _loginService;
        private readonly IhistoryLogService service;
        private readonly ISessionServices _sessionServices;
        private ILogger<userController> _logger;
        private readonly IpAddress _ipAddress;
        private readonly IFileStorageService _ifileStorageService;
        private readonly IFileUploadService _ifileUploadService;
        private readonly IFileTotalServices _ifileTotalServices;
        CultureInfo MainInfo = new CultureInfo("en-GB");
        public userController(IuserService iuserService,
            ILoginService loginService, 
            IhistoryLogService ihistory,
            ISessionServices sessionServices, 
            ILogger<userController> logger,
            IpAddress ipAddress,
            IFileUploadService fileUploadService,
            IFileStorageService fileStorageService,
            IFileTotalServices fileTotalServices
            )
        {
            _iuserService = iuserService;
            _loginService = loginService;
            _sessionServices = sessionServices;
            _logger = logger;
            service = ihistory;
            _ipAddress = ipAddress;
            _ifileStorageService = fileStorageService;
            _ifileUploadService = fileUploadService;
            _ifileTotalServices = fileTotalServices;
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
            if (principal != null)
            {
                if (principal.Login.Suspend && !principal.Admin)
                {
                    ViewBag.result = "Your account is Suspend Please Contact Admin";
                    return View();
                }
                if (!principal.Login.Verify && !principal.Admin)
                {
                    ViewBag.result = "Your account is Unverify Please Contact Admin";
                    return View();
                }
                // System.Web.HttpContext.Current.Session("userId") = principal.Id;
                HttpContext.Session.SetString("uid", principal.Id.ToString());
                HttpContext.Session.SetString("fn", principal.Firstname);
                HttpContext.Session.SetString("ln", principal.Lastname);
                HttpContext.Session.SetString("un", username);
                HttpContext.Session.SetString("sid", _sessionServices.Login(principal.Id,_ipAddress.GetIp()));
                if (principal.Admin)
                {
                    HttpContext.Session.SetString("admin", "1");
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
            _sessionServices.Logout(HttpContext.Session.GetString("sid"));
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
            if (password == retype)
            {
                try
                {
                    string fn, ln, Email, username;
                    DateTime brithday;
                    bool male;
                    fn = HttpContext.Request.Form["Fn"].ToString();
                    ln = HttpContext.Request.Form["Ln"].ToString();
                    Email = HttpContext.Request.Form["Email"].ToString();
                    username = HttpContext.Request.Form["username"].ToString();
                    male = bool.Parse(HttpContext.Request.Form["Gender"].ToString());
                    brithday = DateTime.Parse(HttpContext.Request.Form["BR"].ToString(), new CultureInfo("en-GB"));
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
                catch (Exception x)
                {
                    _logger.LogError(x.Message);
                    ViewBag.result = "Registerd Corruped";
                    return View();
                }

            }
            ViewBag.result = "Password Retype Incorrect";
            return View();
        }

        [HttpGet("/user/Profile")]
        public IActionResult Profile()
        {
         var Hasuser =   long.TryParse(HttpContext.Session.GetString("uid"), out long user);
            if(Hasuser && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
            {
                var userhis = service.GetbyUserAsync(user).GetAwaiter().GetResult();
                ViewBag.h = userhis;
                var currentuser = _iuserService.GetById(user);
                ViewBag.data = currentuser;               
                var CurrentSession = HttpContext.Session.GetString("sid");
                ViewBag.CurrentSession = CurrentSession;
                var allsession = _sessionServices.GetByUser(user);
                ViewBag.a = allsession;
                return View();
            }
            return Redirect("/Home/Restricted");
        }
        [HttpPost("/user/ForcedLogout")]
        public IActionResult ForcedLogout()
        {
            var Hasuser = long.TryParse(HttpContext.Session.GetString("uid"), out long user);
            if (Hasuser && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
            {
                _sessionServices.ForcedClear(user, HttpContext.Session.GetString("sid"));
                return Redirect("/user/Profile");
            }
            return Redirect("/Home/Restricted");
        }
        [HttpGet("/user/Changepassword")]
        public IActionResult Changepass()
        {
            return View();
        }
        [HttpPost("/user/Changepassword")]
        public IActionResult Changingpassres()
        {
            string Oldpass = Request.Form["Old"].ToString();
            string newpass = Request.Form["new"].ToString();
            string username = HttpContext.Session.GetString("un");
            if(!string.IsNullOrEmpty(username))
            {
                var res = _loginService.Changepassword(username, newpass, Oldpass);
                ViewBag.res = res;
                return View();
            }
            return Redirect("/Home/Restricted");
        }

        #region Admin
        [HttpGet("/user/Getunverify")]
        public async Task<IActionResult> GetUnverify()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long user);
            User principal = _iuserService.GetWithoutPassword(user);
            if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
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
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    var res = _iuserService.SetVerifyByadmin(Verifyuser);
                    if (res == 1)
                    {
                        ViewBag.res = "Success";
                    }
                    else if (res == 0)
                    {
                        ViewBag.res = "Unsuccess";
                    }
                    if (res == 2)
                    {
                        return Redirect("/user/Getunverify");
                    }
                    List<User> UnverifyUsers = _iuserService.GetUnverifyAsync().GetAwaiter().GetResult();
                    ViewBag.data = UnverifyUsers;
                    return View("Getunverify");
                }
                return Redirect("/Home/Restricted");
            }
            catch (Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }
        }
        [HttpGet("/user/Suspend")]
        public async Task<IActionResult> Suspend()
        {
            try
            {
                long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
                User principal = _iuserService.GetWithoutPassword(Adminuser);
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    List<User> VerifiedUsers = await _iuserService.GetVerifiedAccountsAsync();
                    ViewBag.data = VerifiedUsers;
                    return View();
                }
                return Redirect("/Home/Restricted");
            }
            catch (Exception x)
            {
                _logger.LogError(x.Message);
                return Redirect("/Home/Restricted");
            }
        }
        [HttpPost("/user/Setsuspend")]
        public IActionResult SetSuspend()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if(principal != null)
            {
                if(principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    long UserId = long.Parse(Request.Form["userId"].ToString());
                    Login claim = _loginService.GetDataByUserId(UserId);
                    _loginService.ChangeSuspendStatus(claim.Username);
                    return Redirect("/user/Suspend");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpGet("/user/Admin/users")]
        public async Task<IActionResult> AdminView()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    ViewBag.Mode = 1;
                    var UserList = await _iuserService.GetViewModelAsync();
                    ViewBag.user = UserList;              
                    return View();
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpPost("/user/Admin/users")]
        public async Task<IActionResult> AdminViewS()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    UserSearchCriteria criteria = new UserSearchCriteria()
                    { Brithday = new dt
                    { Enddate = !string.IsNullOrEmpty(Request.Form["bed"].ToString()) ? DateTime.Parse(Request.Form["bed"].ToString(), MainInfo) : null,
                        Startdate = !string.IsNullOrEmpty(Request.Form["bsd"].ToString()) ? DateTime.Parse(Request.Form["bsd"].ToString(), MainInfo) : null
                    },
                        male = int.Parse(Request.Form["sex"].ToString()),
                        Registerddate = new dt
                        {
                            Enddate = !string.IsNullOrEmpty(Request.Form["red"].ToString()) ? DateTime.Parse(Request.Form["red"].ToString(), MainInfo) : null,
                            Startdate = !string.IsNullOrEmpty(Request.Form["rsd"].ToString()) ? DateTime.Parse(Request.Form["rsd"].ToString(), MainInfo) : null
                        },
                        Spaces = int.Parse(Request.Form["usage"].ToString()),
                        Suspension = int.Parse(Request.Form["sup"].ToString()),
                        Verify  = int.Parse(Request.Form["ver"].ToString()),
                    };
                    ViewBag.Mode = 1;
                    var UserList = await _iuserService.getViewWithAdvancedSearch(criteria);
                    ViewBag.user = UserList;
                    ViewBag.Criteria = criteria;
                    return View("AdminView");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpGet("/user/Admin/history")]
        public async Task<IActionResult> AdminHistory()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    var UserList = await _iuserService.GetViewModelAsync();
                    ViewBag.user = UserList;
                    var History = await service.Getall();                    
                    var vd = await service.GetViewBydate();
                    ViewBag.HistorySummary = vd.Take(10).ToList();
                    ViewBag.history = History;
                    ViewBag.Mode = 2;
                    return View("AdminView");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpPost("/user/Admin/history")]
        public async Task<IActionResult> AdminHistoryP()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    HistoriesCriteria criteria = new HistoriesCriteria
                    {
                        Users= int.TryParse(Request.Form["use"].ToString(), out int ts) ? int.Parse(Request.Form["use"].ToString()):0,
                        Historiesdate= new dt
                        {
                            Enddate=!string.IsNullOrEmpty(Request.Form["ed"].ToString())? DateTime.Parse(Request.Form["ed"].ToString(),MainInfo):null,
                            Startdate= !string.IsNullOrEmpty(Request.Form["sd"].ToString()) ? DateTime.Parse(Request.Form["sd"].ToString(),MainInfo):null
                        },
                        Section=Request.Form["se"].ToString(),
                        State=int.TryParse(Request.Form["sec"].ToString(),out int t) ? int.Parse(Request.Form["sec"].ToString()) :0
                    };
                    var UserList = await _iuserService.GetViewModelAsync();
                    ViewBag.user = UserList;
                    var History = await service.AdvancedSearch(criteria);
                    var vd = await service.GetViewBydate();
                    var sx = await service.Getall();
                    var se = sx.Select(x => x.HistoryMode).Distinct().ToArray();
                    ViewBag.se = se;
                    ViewBag.HistorySummary = vd.Take(10).ToList();
                    ViewBag.history = History;
                    ViewBag.Mode = 2;
                    ViewBag.Criteria = criteria;
                    return View("AdminView");
                }
            }
            return Redirect("/Home/Restricted");
        }
        [HttpPost("/Admin/DeletedHistory")]
        public async Task<IActionResult> Clearhis()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    int month = int.Parse(Request.Form["m"].ToString());
                    int res = await service.Clear(month);
                    return Redirect("/user/Admin/history");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpGet("/user/Admin/Uploads")]
        public async Task<IActionResult> AdminUploads()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    var UserList = await _iuserService.GetViewModelAsync();
                    ViewBag.user = UserList;
                    var allFile = await _ifileTotalServices.GetAsync();
                    ViewBag.Files = allFile;
                    ViewBag.TotalUsed = _ifileStorageService.GetTotalUsedSpace() + _ifileUploadService.GetTotalUsedSpace();
                    ViewBag.Mode = 3;
                    var ext = allFile.Select(x => x.FileExtension).Distinct().ToList();
                    var namespaces = allFile.Select(x => x.FileNamespace).Distinct().ToList();
                    ViewBag.ext = ext;
                    ViewBag.nspaces = namespaces;
                    return View("AdminView");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpPost("/user/Admin/Uploads")]
        public async Task<IActionResult> AdminUploadsP()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    Filecriteria filecriteria = new Filecriteria();

                   filecriteria.AddDateEnd = !string.IsNullOrEmpty(Request.Form["dateend"].ToString()) ? DateTime.Parse(Request.Form["dateend"].ToString(), MainInfo) : null;
                   filecriteria.AddDateStarts = !string.IsNullOrEmpty(Request.Form["datestart"].ToString()) ? DateTime.Parse(Request.Form["datestart"].ToString(), MainInfo) : null;
                   filecriteria.FileNamespace = Request.Form["content"].ToString();
                   filecriteria.FileExtension = Request.Form["ext"].ToString();
                   filecriteria.FileMode = int.Parse(Request.Form["mode"].ToString());
                   filecriteria.HasPassword = Request.Form["Hpass"].ToString() == "1" ? true : false;
                   filecriteria.StatusMode = int.Parse(Request.Form["status"].ToString());
                   filecriteria.UserId = long.Parse(Request.Form["user"].ToString());
                    ViewBag.C = filecriteria;
                    var selectedFiles = await _ifileTotalServices.GetByAdvancedSearch(filecriteria);
                    var UserList = await _iuserService.GetViewModelAsync();
                    ViewBag.user = UserList;
                    var allFile = await _ifileTotalServices.GetAsync();
                    ViewBag.Files = selectedFiles;
                    ViewBag.TotalUsed = _ifileStorageService.GetTotalUsedSpace() + _ifileUploadService.GetTotalUsedSpace();
                    ViewBag.Mode = 3;
                    var ext = allFile.Select(x => x.FileExtension).Distinct().ToList();
                    var namespaces = allFile.Select(x => x.FileNamespace).Distinct().ToList();
                    ViewBag.ext = ext;
                    ViewBag.nspaces = namespaces;
                    return View("AdminView");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpGet("/user/Admin/Sessions")]
        public async Task<IActionResult> AdminSessions()
        {
            long.TryParse(HttpContext.Session.GetString("uid"), out long Adminuser);
            User principal = _iuserService.GetWithoutPassword(Adminuser);
            if (principal != null)
            {
                if (principal.Admin && _sessionServices.Sessioncheck(HttpContext.Session.GetString("sid")))
                {
                    var UserList = await _iuserService.GetViewModelAsync();
                    ViewBag.user = UserList;
                    var Sessions = await _sessionServices.GetallAsync();
                    ViewBag.S = Sessions;
                    ViewBag.Mode = 4;
                    return View("AdminView");
                }
            }
            return Redirect("/Home/Restricted");
        }
        #endregion



    }
}
