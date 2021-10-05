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

namespace TestUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IuserService _iuserService;
        private readonly IFileUploadService _ifileUploadService;
        private readonly IFileStorageService _ifileStorageService;

        public HomeController(ILogger<HomeController> logger,
            IuserService iuserService,
            IFileStorageService fileStorageService,
            IFileUploadService fileUploadService
            )
        {
            _logger = logger;
            _iuserService = iuserService;
            _ifileStorageService = fileStorageService;
            _ifileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var i = await _iuserService.GetallUsersAsync();
            var f = await _ifileUploadService.GetAllFileUploadsAsync();
            var s = await _ifileStorageService.GetAllFileStoragesAsync();

            ViewBag.Totaluser = i.Count;
            ViewBag.TotalFiles = f.Count;
            ViewBag.TotalBlobs = s.Count;


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
    }
}
