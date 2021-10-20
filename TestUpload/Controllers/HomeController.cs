using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models;
using TestUpload.Service;
using TestUpload.Models.Entity;
using Microsoft.AspNetCore.Http;

namespace TestUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IuserService _iuserService;
        private readonly IFileUploadService _ifileUploadService;
        private readonly IFileStorageService _ifileStorageService;
        private readonly ISessionServices _isessionServices;

        public HomeController(ILogger<HomeController> logger,
            IuserService iuserService,
            IFileStorageService fileStorageService,
            IFileUploadService fileUploadService,
            ISessionServices sessionServices
            )
        {
            _logger = logger;
            _iuserService = iuserService;
            _ifileStorageService = fileStorageService;
            _ifileUploadService = fileUploadService;
            _isessionServices = sessionServices;
        }

        public async Task<IActionResult> Index()
        {
            var i = await _iuserService.GetallUsersAsync();
            var f = await _ifileUploadService.GetAllFileUploadsAsync();
            var s = await _ifileStorageService.GetAllFileStoragesAsync();

            ViewBag.Totaluser = i.Count;
            ViewBag.TotalFiles = f.Count;
            ViewBag.TotalBlobs = s.Count;
           
            if(!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                var session = HttpContext.Session.GetString("sid");
                var t = _isessionServices.Sessioncheck(session);
                if (!t)
                {
                    HttpContext.Session.Clear();
                    return Redirect("/");
                }
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                var Myfile = await _ifileUploadService.GetFilesByUserAsync(user);
                var Mystorage = await _ifileStorageService.GetFilesByUserAsync(user);
                ViewBag.Files = Myfile.Count();
                ViewBag.Storage = Mystorage.Count();
                var j = Myfile.Select(x => x.FileSize).Sum() + Mystorage.Select(x => x.FileSize).Sum();          
                if (j <= 1024)
                {
                    ViewBag.space = j.ToString("0.00") + " Bytes";
                }
                else if (j <= (1024 * 1024))
                {
                    ViewBag.space = (j / 1024).ToString("0.00") + " KB";
                }
                else if (j <= (1024 * 1024 * 1024))
                {
                    ViewBag.space = (j / (1024 * 1024)).ToString("0.00") + " MB";
                }
                else
                {
                    ViewBag.space = (j / (1024 * 1024 * 1024)).ToString("0.00") + " GB";
                }

            }


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet("/Home/Restricted")]
        public IActionResult Rest()
        {
            return View();
        }
    }
}
