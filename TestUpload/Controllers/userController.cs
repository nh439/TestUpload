using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Controllers
{
    public class userController : Controller
    {
        [HttpGet("/user")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
