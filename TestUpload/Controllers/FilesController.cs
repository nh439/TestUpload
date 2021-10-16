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
using Microsoft.Extensions.Logging;

namespace TestUpload.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileUploadService IfileUploadService;
        private readonly IFileStorageService IfileStorageService;
        private readonly IConfiguration Iconfiguration;
        private readonly IhistoryLogService service;
        private readonly ILoginService IloginService;
        private readonly ILogger<FilesController> _logger;
        CultureInfo THinfo = new CultureInfo("en-GB");
        public FilesController(IFileUploadService fileUploadService, IFileStorageService fileStorageService, IConfiguration configuration, IhistoryLogService ihistory,ILoginService loginService, ILogger<FilesController> logger)
        {
            IfileUploadService = fileUploadService;
            IfileStorageService = fileStorageService;
            Iconfiguration = configuration;
            service = ihistory;
            IloginService = loginService;
            _logger = logger;
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
                              Contentspace = f.Uploadname
                          }).ToList();
                var db = (from f in storage
                          select new
                          {
                              Extension = f.FileExtension,
                              Contentspace = f.Uploadname
                          }).ToList();

                var dc = da.Union(db);
                ViewBag.Extension = dc.Select(x => x.Extension).Distinct().ToList();
                ViewBag.Ctype = dc.Select(x => x.Contentspace).Distinct().ToList();
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
                    AddDateEnd = !string.IsNullOrEmpty(Request.Form["dateend"].ToString()) ?  DateTime.Parse(Request.Form["dateend"].ToString(),THinfo) : null,
                    AddDateStarts = !string.IsNullOrEmpty(Request.Form["datestart"].ToString()) ?  DateTime.Parse(Request.Form["datestart"].ToString(),THinfo) : null,
                    FileNamespace = Request.Form["content"].ToString(),
                    FileExtension = Request.Form["ext"].ToString(),
                    FileMode = int.Parse(Request.Form["mode"].ToString()),
                    HasPassword = Request.Form["Hpass"].ToString() == "1" ? true:false ,
                    StatusMode=int.Parse(Request.Form["status"].ToString())
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
                              Contentspace = f.Uploadname
                          }).ToList();
                var db = (from f in storageA
                          select new
                          {
                              Extension = f.FileExtension,
                              Contentspace = f.Uploadname
                          }).ToList();

                var dc = da.Union(db);
                var namespaces = dc.Select(x => x.Contentspace).Distinct().ToList();
                var idx = namespaces.FindIndex(x => string.IsNullOrEmpty(x));
                if (idx >= 0)
                {
                    namespaces[idx] = "[No Namespaces]";
                }
                ViewBag.Extension = dc.Select(x => x.Extension).Distinct().ToList();
                ViewBag.Ctype = namespaces;
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
                    string uploadId = Guid.NewGuid().ToString();
                    bool share = Request.Form["shared"].ToString() == "true" ? true:false;
                    string nsp = Request.Form["nspec"].ToString();


                    PasswordHash passwordHash = new PasswordHash();
                    if (blob)
                    
                    {
                        List<FileStorage> uploads = new List<FileStorage>();
                        foreach (var att in Files)
                        {
                            FileStorage fileUpload = new FileStorage()
                            {
                                AddDate =DateTime.Now,
                                Comment = Request.Form["com"].ToString(),
                                FileExtension = Path.GetExtension(att.FileName),
                                Filename = Path.GetFileNameWithoutExtension(att.FileName),
                                FileSize = att.Length,
                                FileType = att.ContentType,
                                UploadId=uploadId,
                                Shared=share,
                                Uploadname=nsp,
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
                                UploadId=uploadId,
                                Shared=share,
                                Uploadname=nsp,
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
                    _logger.LogError(x.Message);
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
            catch(Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500,x.Message);
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
            catch(Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
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
                    _logger.LogError(x.Message);
                    return StatusCode(500, x.Message);
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
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }

        }
        [HttpPost("Files/Reset")]
        public IActionResult Setpass()
        {
            var data = new
            {
                Id = Request.Form["id"].ToString(),
                Blob = bool.Parse(Request.Form["blob"].ToString())
            };
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")) )
            {
                ViewBag.Id = data.Id;
                ViewBag.Blob = data.Blob ? true:false;
                return View();
            }
            return Redirect("/Home/Restricted");
        }

        [HttpPost("Files/Resetpass")]
        public IActionResult Resetpass()
        {
            try
            {
                bool blob = bool.Parse(Request.Form["Blob"].ToString());
                string Id = Request.Form["id"].ToString();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
                {
                    string pass = Request.Form["upass"].ToString();
                    string username = HttpContext.Session.GetString("un");
                    long user = long.Parse(HttpContext.Session.GetString("uid"));
                    var userVerify = IloginService.GetLogin(username, pass);
                    if (userVerify != null)
                    {
                        if (blob)
                        {
                            var datacheck = IfileStorageService.GetViewById(Id).GetAwaiter().GetResult();
                            if (datacheck.UserId == user)
                            {
                                string newpass = Request.Form["newpass"].ToString();
                                IfileStorageService.Setpassword(Id, newpass);
                                return Redirect("/Files");
                            }
                            return Redirect("/Home/Restricted");
                        }
                        else
                        {
                            var datacheck = IfileUploadService.GetById(Id);
                            if (datacheck.UserId == user)
                            {
                                string newpass = Request.Form["newpass"].ToString();
                                IfileUploadService.Setpassword(Id, newpass);
                                return Redirect("/Files");
                            }
                            return Redirect("/Home/Restricted");
                        }

                    }
                    ViewBag.Id = Id;
                    ViewBag.Blob = blob;
                    return View();
                }
                return Redirect("/Home/Restricted");
            }
            catch(Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
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
            try
            {
                string FileId = Request.Form["id"].ToString();
                long.TryParse(HttpContext.Session.GetString("uid"), out long user);
                FileStorage Storage = IfileStorageService.Download(FileId);
                MemoryStream ms = new MemoryStream(Storage.RawData);
                ms.Position = 0;
                return File(ms, Storage.FileType, Storage.Filename + Storage.FileExtension);
            }
            catch(Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }
        }
        [HttpPost("/Blob/DownloadV")]
        public IActionResult DownloadVerifyB()
        {
            try
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
            catch(Exception x)
            {
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }
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
                    _logger.LogError(x.Message);
                    return StatusCode(500, x.Message);
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
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
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
                    if (Directory.Exists(Path.GetDirectoryName(filepath)))
                    {
                        Directory.Delete(filepath, true);               
                    }
                    await IfileUploadService.FormatAsync(user);                  
                    await IfileStorageService.FormatAsync(user); 
                    return Redirect("/Files");
                }

                return View();
            }

            return Redirect("/Home/Restricted");
        }
        [HttpPost("/Files/Setting")]
        public IActionResult Setting()
        {
            try
            {
                string namespaces = Request.Form["nspec"].ToString();
                string reference = Request.Form["id"].ToString();
                bool shared = Request.Form["Shared"].ToString()=="true" ? true:false;
                bool blob = bool.Parse(Request.Form["Blob"].ToString());
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("uid")))
                {
                    long user = long.Parse(HttpContext.Session.GetString("uid"));
                    if(blob)
                    {
                        var checkdata = IfileStorageService.GetViewById(reference).GetAwaiter().GetResult();
                        if(checkdata.UserId==user)
                        {
                            IfileStorageService.SetnamespaceAndShared(reference, namespaces, shared);
                            service.CreateSuccessHistory("Setting File ", "Changes Setting", checkdata.Filename + checkdata.FileExtension, user);
                            return Redirect("/Files");
                        }
                    }
                    else
                    {
                        var checkdata = IfileUploadService.GetById(reference);
                        if (checkdata.UserId == user)
                        {
                            IfileUploadService.SetnamespaceAndShared(reference, namespaces, shared);
                            service.CreateSuccessHistory("Setting File ", "Changes Setting", checkdata.Filename + checkdata.FileExtension, user);
                            return Redirect("/Files");
                        }
                    }

                }
                return Redirect("/Home/Restricted");
            }
            catch (Exception x)
            {
                service.CreateErrorHistory("Setting File ", "Changes Setting Failed",null,long.TryParse(HttpContext.Session.GetString("uid"),out long UserId) ? UserId:0,x.Message,null);
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }

        }
        [HttpGet("Share/{B}/{Token}")]
        public IActionResult PublicDownload(string B,string Token)
        {
            try
            {
                bool blob = int.Parse(B) == 1 ? true : false;
                if(blob)
                {
                    var item = IfileStorageService.GetByToken(Token);                    
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(item.pass))
                        {
                            ViewBag.Blob = blob ? 1 : 0;
                            ViewBag.Token = Token;
                            ViewBag.Id = item.Id;
                            ViewBag.Filenames = item.Filename + item.FileExtension;
                            return View();
                        }
                        MemoryStream ms = new MemoryStream(item.RawData);
                        return File(ms, item.FileType, item.Filename + item.FileExtension);
                    }
                }
                else
                {
                    var item = IfileUploadService.GetByToken(Token);                 
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(item.pass))
                        {
                            ViewBag.Blob = blob ? 1:0;
                            ViewBag.Token = Token;
                            ViewBag.Id = item.Id;
                            ViewBag.Filenames = item.Filename + item.FileExtension;
                            return View();
                        }
                        var filepath = Iconfiguration.GetSection("att").Value;
                        filepath = filepath + "\\" + item.UserId + "\\" + item.Id;
                        MemoryStream ms = new MemoryStream();
                        FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                        stream.CopyTo(ms);
                        stream.Close();
                        ms.Position = 0;
                        return File(ms, item.FileType, item.Filename + item.FileExtension);
                    }

                }
                return Content("<h1> File Not Found </h1>");
            }
            catch (Exception x)
            {
                // service.CreateErrorHistory("Upload Files", "Set Me Unsuccessful", "", user, x.Message, x.InnerException.Message);
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }                     
        }
        [HttpPost("Share/Download")]
        public IActionResult PublicVerify()
        {
            try
            {
                bool blob = int.Parse(Request.Form["Blob"].ToString()) == 1 ? true : false;
                string Token = Request.Form["Token"].ToString();
                string Id = Request.Form["id"].ToString();
                string password = Request.Form["filepass"].ToString();
                PasswordHash hash = new PasswordHash();
                password = hash.CreateEncrypted(Id, password);
                if (blob)
                {
                    var item = IfileStorageService.GetByToken(Token);
                    if (password == item.pass)
                    {
                        MemoryStream ms = new MemoryStream(item.RawData);
                        return File(ms, item.FileType, item.Filename + item.FileExtension);
                    }
                    ViewBag.Blob = blob ? 1 : 0;
                    ViewBag.Token = Token;
                    ViewBag.Id = item.Id;
                    ViewBag.Filenames = item.Filename + item.FileExtension;
                    return View();
                }
                else
                {
                    var item = IfileUploadService.GetByToken(Token);
                    if (password == item.pass)
                    {
                        var filepath = Iconfiguration.GetSection("att").Value;
                        filepath = filepath + "\\" + item.UserId + "\\" + item.Id;
                        MemoryStream ms = new MemoryStream();
                        FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                        stream.CopyTo(ms);
                        stream.Close();
                        ms.Position = 0;
                        return File(ms, item.FileType, item.Filename + item.FileExtension);
                    }
                    ViewBag.Blob = blob ? 1 : 0;
                    ViewBag.Token = Token;
                    ViewBag.Id = item.Id;
                    ViewBag.Filenames = item.Filename + item.FileExtension;
                    return View();
                }
                return Redirect("/Home/Restricted");
            }
            catch (Exception x)
            {
                //  service.CreateErrorHistory("Upload Files", "Set Me Unsuccessful", "", user, x.Message, x.InnerException.Message);
                _logger.LogError(x.Message);
                return StatusCode(500, x.Message);
            }
        }

    }
}
