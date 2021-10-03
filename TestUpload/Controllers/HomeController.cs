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

        public HomeController(ILogger<HomeController> logger,IuserService iuserService)
        {
            _logger = logger;
            _iuserService = iuserService;
        }

        public async Task<IActionResult> Index()
        {
            var i = await _iuserService.GetallUsersAsync();
            ViewBag.Totaluser = i.Count;

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
