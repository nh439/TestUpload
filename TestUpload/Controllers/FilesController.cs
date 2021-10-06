using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using TestUpload.Service;
using TestUpload.Models.Entity;
using TestUpload.Models.View;

namespace TestUpload.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileUploadService IfileUploadService ;
        private readonly IFileStorageService IfileStorageService;
        public FilesController(IFileUploadService fileUploadService,IFileStorageService fileStorageService)
        {
            IfileUploadService = fileUploadService;
            IfileStorageService = fileStorageService;
        }


        [HttpGet("/Files")]
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                List<FileUpload> files = IfileUploadService.GetFilesByUserAsync(user).GetAwaiter().GetResult();
                ViewBag.files = files;
                List<FilestorageView> storage = IfileStorageService.GetFilesByUserAsync(user).GetAwaiter().GetResult();
                ViewBag.storage = storage;
                return View();
            }
            return Redirect("/");
        }
        [HttpGet("/Files/Uploads")]
        public IActionResult Uploads()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                return View();
            }
            return Redirect("/");
        }

    }
}
