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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using TestUpload.Securities;

namespace TestUpload.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileUploadService IfileUploadService ;
        private readonly IFileStorageService IfileStorageService;
        private readonly IConfiguration Iconfiguration;
        private readonly IhistoryLogService service;
        public FilesController(IFileUploadService fileUploadService,IFileStorageService fileStorageService,IConfiguration configuration,IhistoryLogService ihistory)
        {
            IfileUploadService = fileUploadService;
            IfileStorageService = fileStorageService;
            Iconfiguration = configuration;
            service = ihistory;
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
        [HttpPost("/Files/Uploads")]
        public async Task<IActionResult> Uploaded(List<IFormFile> Files)
        { 
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                try
                {
                    bool blob = int.Parse(Request.Form["Storage"].ToString()) == 1 ? true : false;
                    var filepath = Iconfiguration.GetSection("att").Value;
                    

                    PasswordHash passwordHash = new PasswordHash();
                    if (blob)
                    {

                    }
                    else
                    {
                        List<FileUpload> uploads = new List<FileUpload>();
                        foreach (var att in Files)
                        {
                            FileUpload fileUpload = new FileUpload()
                            {
                                AddDate = DateTime.Now,
                                Comment = Request.Form["com"].ToString(),
                                FileExtension = Path.GetExtension(att.FileName),
                                Filename = Path.GetFileNameWithoutExtension(att.FileName),
                                FileSize = att.Length,
                                LastUpdate = DateTime.Now,

                                UserId = user
                            };
                            fileUpload.pass = !string.IsNullOrEmpty(Request.Form["pass"].ToString()) ? passwordHash.CreateEncrypted(fileUpload.Id, Request.Form["pass"].ToString()) : string.Empty;
                            var savepath = Path.Combine(filepath, user.ToString(), fileUpload.Id);

                            MemoryStream Inputstream = new MemoryStream();
                            await att.CopyToAsync(Inputstream);
                            byte[] br = Inputstream.ToArray();

                            if (!Directory.Exists(Path.GetDirectoryName(savepath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(savepath));
                            }
                            FileStream OutputStream = new FileStream(savepath, FileMode.Create, FileAccess.Write);
                            await OutputStream.WriteAsync(br, 0, br.Length);
                            OutputStream.Close();
                            uploads.Add(fileUpload);

                        }
                        var res = await IfileUploadService.CreateAsync(uploads);
                        foreach (var i in uploads)
                        {
                            service.CreateSuccessHistory("Upload Files", "Upload File Successful", i.Filename + i.FileExtension, user);
                        }
                        ViewBag.Issuccess = true;
                        return View();
                    }

                    return Ok();
                }
                catch(Exception x)
                {
                    ViewBag.Issuccess = false;
                    service.CreateErrorHistory("Upload Files", "Upload File Unsuccessful", "",user, x.Message, x.InnerException.Message);
                    return View();
                }
            }
            return Redirect("/");
        }

    }
}
