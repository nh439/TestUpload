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
using TestUpload.Models.criteria;
using System.Globalization;

namespace TestUpload.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileUploadService IfileUploadService;
        private readonly IFileStorageService IfileStorageService;
        private readonly IConfiguration Iconfiguration;
        private readonly IhistoryLogService service;
        private readonly ILoginService IloginService;
        CultureInfo THinfo = new CultureInfo("th-TH");
        public FilesController(IFileUploadService fileUploadService, IFileStorageService fileStorageService, IConfiguration configuration, IhistoryLogService ihistory,ILoginService loginService)
        {
            IfileUploadService = fileUploadService;
            IfileStorageService = fileStorageService;
            Iconfiguration = configuration;
            service = ihistory;
            IloginService = loginService;
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
                var da = (from f in files
                          select new
                          {
                              Extension = f.FileExtension,
                              ContentType = f.FileType
                          }).ToList();
                var db = (from f in storage
                          select new
                          {
                              Extension = f.FileExtension,
                              ContentType = f.FileType
                          }).ToList();

                var dc = da.Union(db);
                ViewBag.Extension = dc.Select(x => x.Extension).Distinct().ToList();
                ViewBag.Ctype = dc.Select(x => x.ContentType).Distinct().ToList();
                var j = files.Select(x => x.FileSize).Sum() + storage.Select(x => x.FileSize).Sum();
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
                return View();
            }
            return Redirect("/");
        }
        [HttpPost("/Files")]
        public IActionResult Index1()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                Filecriteria filecriteria = new Filecriteria
                {
                    AddDateEnd = !string.IsNullOrEmpty(Request.Form["dateend"].ToString()) ? DateConvert.GetBuddhist_era( DateTime.Parse(Request.Form["dateend"].ToString())) : null,
                    AddDateStarts = !string.IsNullOrEmpty(Request.Form["datestart"].ToString()) ? DateConvert.GetBuddhist_era( DateTime.Parse(Request.Form["datestart"].ToString())) : null,
                    Contentype = Request.Form["content"].ToString(),
                    FileExtension = Request.Form["ext"].ToString(),
                    FileMode = int.Parse(Request.Form["mode"].ToString()),
                    HasPassword = Request.Form["Hpass"].ToString() == "1" ? true:false 
                };
                ViewBag.CR = filecriteria;
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                List<FileUpload> files = IfileUploadService.GetFilescriteria(user, filecriteria).GetAwaiter().GetResult();
                ViewBag.files = files;
                List<FilestorageView> storage = IfileStorageService.GetFilescriteria(user, filecriteria).GetAwaiter().GetResult();
                ViewBag.storage = storage;
                List<FileUpload> filesA = IfileUploadService.GetFilesByUserAsync(user).GetAwaiter().GetResult();
                List<FilestorageView> storageA = IfileStorageService.GetFilesByUserAsync(user).GetAwaiter().GetResult();
                var da = (from f in filesA
                          select new
                          {
                              Extension = f.FileExtension,
                              ContentType = f.FileType
                          }).ToList();
                var db = (from f in storageA
                          select new
                          {
                              Extension = f.FileExtension,
                              ContentType = f.FileType
                          }).ToList();

                var dc = da.Union(db);
                ViewBag.Extension = dc.Select(x => x.Extension).Distinct().ToList();
                ViewBag.Ctype = dc.Select(x => x.ContentType).Distinct().ToList();
                var j = files.Select(x => x.FileSize).Sum() + storage.Select(x => x.FileSize).Sum();
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
                        List<FileStorage> uploads = new List<FileStorage>();
                        foreach (var att in Files)
                        {
                            FileStorage fileUpload = new FileStorage()
                            {
                                AddDate = DateTime.Now,
                                Comment = Request.Form["com"].ToString(),
                                FileExtension = Path.GetExtension(att.FileName),
                                Filename = Path.GetFileNameWithoutExtension(att.FileName),
                                FileSize = att.Length,
                                FileType = att.ContentType,

                                UserId = user
                            };
                            fileUpload.pass = !string.IsNullOrEmpty(Request.Form["pass"].ToString()) ? passwordHash.CreateEncrypted(fileUpload.Id, Request.Form["pass"].ToString()) : string.Empty;
                            var savepath = Path.Combine(filepath, user.ToString(), fileUpload.Id);
                            MemoryStream Inputstream = new MemoryStream();
                            await att.CopyToAsync(Inputstream);
                            byte[] br = Inputstream.ToArray();
                            fileUpload.RawData = br;
                            uploads.Add(fileUpload);

                        }
                        var res = await IfileStorageService.CreateAsync(uploads);
                        foreach (var i in uploads)
                        {
                            service.CreateSuccessHistory("Upload Files", "Upload File Successful", i.Filename + i.FileExtension, user);
                        }
                        ViewBag.Issuccess = true;
                        return View();
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
                                FileType = att.ContentType,

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
                catch (Exception x)
                {
                    ViewBag.Issuccess = false;
                    service.CreateErrorHistory("Upload Files", "Upload File Unsuccessful", "", user, x.Message, x.InnerException.Message);
                    return View();
                }
            }
            return Redirect("/");
        }
        [HttpGet("/Files/Download/{id}")]
        public IActionResult DownloadF(string id)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                FileUpload data = IfileUploadService.GetById(id);
                if (data.UserId == user)
                {
                    ViewBag.data = data;
                    var j = data.FileSize;
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
                    return View();
                }
            }
            return Redirect("/Home/Restricted");

        }
        [HttpPost("/Files/Download")]
        public async Task<IActionResult> Downloading()
        {
            try
            {
                string FileId = Request.Form["id"].ToString();
                long.TryParse(HttpContext.Session.GetString("uid"), out long user);
                FileUpload fileUpload = IfileUploadService.Download(FileId);
                var filepath = Iconfiguration.GetSection("att").Value;
                filepath = filepath + "\\" + user + "\\" + fileUpload.Id;
                MemoryStream ms = new MemoryStream();
                FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                await stream.CopyToAsync(ms);
                stream.Close();
                ms.Position = 0;
                return File(ms, fileUpload.FileType, fileUpload.Filename + fileUpload.FileExtension);
            }
            catch
            {
                return Content("Something Went Wrong");
            }
        }
        [HttpPost("/Files/DownloadV")]
        public async Task<IActionResult> DownloadVerify()
        {
            try
            {
                PasswordHash hash = new PasswordHash();
                string FileId = Request.Form["id"].ToString();
                string password = Request.Form["pass"].ToString();
                password = hash.CreateEncrypted(FileId, password);
                long.TryParse(HttpContext.Session.GetString("uid"), out long user);
                FileUpload fileUpload = IfileUploadService.VerifyDownload(FileId, password);
                if (fileUpload == null)
                {
                    return View();
                }
                var filepath = Iconfiguration.GetSection("att").Value;
                filepath = filepath + "\\" + user + "\\" + fileUpload.Id;
                MemoryStream ms = new MemoryStream();
                FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                await stream.CopyToAsync(ms);
                stream.Close();
                ms.Position = 0;
                return File(ms, fileUpload.FileType, fileUpload.Filename + fileUpload.FileExtension);
            }
            catch
            {
                return Content("Something Went Wrong");
            }
        }

        [HttpGet("/Files/Remove/{id}")]
        public IActionResult RemoveFile(string id)
        {
            FileUpload fileUpload = IfileUploadService.GetById(id);
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")) && fileUpload.UserId == long.Parse(HttpContext.Session.GetString("uid")))
            {
                var user = long.Parse(HttpContext.Session.GetString("uid"));
                try
                {
                    var filepath = Iconfiguration.GetSection("att").Value;
                    filepath = filepath + "\\" + user + "\\" + fileUpload.Id;
                    System.IO.File.Delete(filepath);
                    IfileUploadService.DeleteOne(id);
                    service.CreateSuccessHistory("Upload Files", "Deleted File Successful", fileUpload.Filename + fileUpload.FileExtension, user);
                    return Redirect("/Files");
                }
                catch (Exception x)
                {
                    service.CreateErrorHistory("Upload Files", "Deleted File Failed", "", user, x.Message, x.InnerException.Message);
                    return Content("Delete Error");
                }
            }
            return Redirect("/Home/Restricted");
        }
        [HttpGet("/Files/Verifyremove/{id}")]
        public IActionResult Verifyremoved(string Id)
        {
            FileUpload fileUpload = IfileUploadService.GetById(Id);
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")) && fileUpload.UserId == long.Parse(HttpContext.Session.GetString("uid")))
            {
                ViewBag.file = fileUpload;
                return View();
            }
            return Redirect("/Home/Restricted");
        }
        [HttpPost("/Files/Verifyremove")]
        public IActionResult RemoveVerify()
        {
            var user = long.Parse(HttpContext.Session.GetString("uid"));
            try
            {
                PasswordHash hash = new PasswordHash();
                string Id = Request.Form["id"].ToString();
                string password = Request.Form["pass"].ToString();
                password = hash.CreateEncrypted(Id, password);
                var i = IfileUploadService.GetById(Id);
                var res = IfileUploadService.VerifyRemove(Id, password);
                if (res)
                {


                    var filepath = Iconfiguration.GetSection("att").Value;
                    filepath = filepath + "\\" + user + "\\" + Id;
                    System.IO.File.Delete(filepath);
                    service.CreateSuccessHistory("Upload Files", "Deleted File Failed", i.Filename + i.FileExtension, user);
                    return Redirect("/Files");
                }
                return View();
            }
            catch (Exception x)
            {
                service.CreateErrorHistory("Upload Files", "Deleted File Successful", "", user, x.Message, x.InnerException.Message);
                return Content("Delete Error");
            }

        }

        [HttpGet("/Blob/Download/{id}")]
        public IActionResult DownloadB(string id)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                FilestorageView data = IfileStorageService.GetViewById(id).GetAwaiter().GetResult();
                if (data.UserId == user)
                {
                    ViewBag.data = data;
                    var j = data.FileSize;
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
                    return View();
                }
            }
            return Redirect("/Home/Restricted");

        }



        [HttpPost("/Blob/Download")]
        public IActionResult DownloadingB()
        {
            string FileId = Request.Form["id"].ToString();
            long.TryParse(HttpContext.Session.GetString("uid"), out long user);
            FileStorage Storage = IfileStorageService.Download(FileId);
            MemoryStream ms = new MemoryStream(Storage.RawData);
            ms.Position = 0;
            return File(ms, Storage.FileType, Storage.Filename + Storage.FileExtension);
        }
        [HttpPost("/Blob/DownloadV")]
        public IActionResult DownloadVerifyB()
        {
            PasswordHash hash = new PasswordHash();
            string FileId = Request.Form["id"].ToString();
            string password = Request.Form["pass"].ToString();
            password = hash.CreateEncrypted(FileId, password);
            long.TryParse(HttpContext.Session.GetString("uid"), out long user);
            FileStorage Storage = IfileStorageService.VerifyDownload(FileId, password);
            if (Storage == null)
            {
                return View();
            }
            MemoryStream ms = new MemoryStream(Storage.RawData);
            ms.Position = 0;
            return File(ms, Storage.FileType, Storage.Filename + Storage.FileExtension);
        }

        [HttpGet("/Blob/Remove/{id}")]
        public IActionResult RemoveBlob(string id)
        {
            FilestorageView StorageView = IfileStorageService.GetViewById(id).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")) && StorageView.UserId == long.Parse(HttpContext.Session.GetString("uid")))
            {
                var user = long.Parse(HttpContext.Session.GetString("uid"));
                try
                {
                    IfileStorageService.DeleteOne(id);
                    service.CreateSuccessHistory("Upload Files", "Deleted File Successful", StorageView.Filename + StorageView.FileExtension, user);
                    return Redirect("/Files");
                }
                catch (Exception x)
                {
                    service.CreateErrorHistory("Upload Files", "Deleted File Failed", "", user, x.Message, x.InnerException.Message);
                    return Content("Delete Error");
                }
            }
            return Redirect("/Home/Restricted");
        }

        [HttpGet("/Blob/Verifyremove/{id}")]
        public IActionResult VerifyremovedB(string Id)
        {
            FilestorageView StorageView = IfileStorageService.GetViewById(Id).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")) && StorageView.UserId == long.Parse(HttpContext.Session.GetString("uid")))
            {
                ViewBag.file = StorageView;
                return View();
            }
            return Redirect("/Home/Restricted");
        }
        [HttpPost("/Blob/Verifyremove")]
        public IActionResult RemoveVerifyB()
        {
            var user = long.Parse(HttpContext.Session.GetString("uid"));
            try
            {
                PasswordHash hash = new PasswordHash();
                string Id = Request.Form["id"].ToString();
                string password = Request.Form["pass"].ToString();
                password = hash.CreateEncrypted(Id, password);
                var i = IfileStorageService.GetViewById(Id).GetAwaiter().GetResult();
                var res = IfileStorageService.VerifyRemove(Id, password);
                if (res)
                {
                    service.CreateSuccessHistory("Upload Files", "Deleted File Successful", i.Filename + i.FileExtension, user);
                    return Redirect("/Files");
                }
                return View();
            }
            catch (Exception x)
            {
                service.CreateErrorHistory("Upload Files", "Deleted File Unsuccessful", "", user, x.Message, x.InnerException.Message);
                return Content("Delete Error");
            }

        }
        [HttpPost("/Files/Format")]
        public async Task<IActionResult> Format()
        {
            if(!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
            {
                PasswordHash hash = new PasswordHash();
                long user = long.Parse(HttpContext.Session.GetString("uid"));
                string password = Request.Form["pass"].ToString();
                string username = HttpContext.Session.GetString("un");
               // password = hash.Create(username, password);
                var Hasuser = IloginService.GetLogin(username, password);
                if (Hasuser != null)
                {
                    var filepath = Iconfiguration.GetSection("att").Value;
                    filepath = filepath + "\\" + user;
                    Directory.Delete(filepath,true);                  
                        await IfileUploadService.FormatAsync(user);                  
                        await IfileStorageService.FormatAsync(user); 
                    return Redirect("/Files");
                }

                return new JavaScriptResult("alert('Password Incorrect'); window.location.href='/Files'");
            }

            return Redirect("/Home/Restricted");
        }
        public class JavaScriptResult : ContentResult
        {
            public JavaScriptResult(string script)
            {
                this.Content = script;
                this.ContentType = "application/javascript";
            }
        }


    }
}
