using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.criteria;
using TestUpload.Models.Entity;
using TestUpload.Repository.SQL;
using TestUpload.Securities;

namespace TestUpload.Service
{
    public interface IFileUploadService
    {
        Task<int> CreateAsync(List<FileUpload> files);
        Task<List<FileUpload>> GetAllFileUploadsAsync();
        Task<List<FileUpload>> GetFilesByUserAsync(long user);
        FileUpload Download(string reference);
        FileUpload VerifyDownload(string reference, string password);
        int Delete(List<string> references);
        bool DeleteOne(string references);
        bool VerifyRemove(string reference, string password);
        FileUpload GetById(string reference);
        Task<List<FileUpload>> GetFilescriteria(long user, Filecriteria filecriteria);
        Task<int> FormatAsync(long user);
        bool Setpassword(string Id, string Newpassword);
        bool SetnamespaceAndShared(string reference, string namespaces, bool shared);
        FileUpload GetByToken(string Token);
        decimal GetTotalUsedSpace();
    }
    public class FileUploadService:IFileUploadService
    {
        private readonly FileUploadRepository _fileUploadRepository;
        public FileUploadService(FileUploadRepository fileUploadRepository)
        {
            _fileUploadRepository = fileUploadRepository;
        }
        public async Task<int> CreateAsync(List<FileUpload> files)
        {
            return await _fileUploadRepository.Create(files);
        }
        public async Task<List<FileUpload>> GetAllFileUploadsAsync()
        {
            return await _fileUploadRepository.Getall();
        }
        public async Task<List<FileUpload>> GetFilesByUserAsync(long user)
        {
            return await _fileUploadRepository.GetByUser(user);
        }
        public FileUpload Download(string reference)
        {
            return  _fileUploadRepository.GetFile(reference);
        }
        public FileUpload GetById(string reference)
        {
            return _fileUploadRepository.GetFileByRef(reference);
        }
        public FileUpload VerifyDownload(string reference,string password)
        {
            return _fileUploadRepository.GetFile(reference,password);
        }
        public int Delete(List<string> references)
        {
            return _fileUploadRepository.Remove(references);
        }
        public bool DeleteOne(string references)
        {
            return _fileUploadRepository.RemoveOne(references);
        }

        public bool VerifyRemove(string reference, string password)
        {
            return _fileUploadRepository.VerifyRemoved(reference, password);
        }

        public async Task<List<FileUpload>> GetFilescriteria(long user, Filecriteria filecriteria)
        {
            var data = await _fileUploadRepository.GetByUser(user);
            if (!string.IsNullOrEmpty(filecriteria.FileExtension))
            {
                data = data.Where(x => x.FileExtension == filecriteria.FileExtension).ToList();
            }
            if (!string.IsNullOrEmpty(filecriteria.FileNamespace))
            {
                if (filecriteria.FileNamespace == "[No Namespaces]")
                {
                    data = data.Where(x => x.Uploadname == string.Empty).ToList();
                }
                else
                {
                    data = data.Where(x => x.Uploadname == filecriteria.FileNamespace).ToList();
                }
            }
           
            if (filecriteria.HasPassword)
            {
                data = data.Where(x => !string.IsNullOrEmpty(x.pass)).ToList();
            }
            if (filecriteria.AddDateStarts.HasValue)
            {
                data = data.Where(x => x.AddDate > filecriteria.AddDateStarts.Value.Date).ToList();
            }
            if (filecriteria.AddDateEnd.HasValue)
            {
                data = data.Where(x => x.AddDate <= filecriteria.AddDateEnd.Value.Date.AddDays(1)).ToList();
            }
            if(filecriteria.StatusMode==1)
            {
                data = data.Where(x => !x.Shared).ToList();
            }
            if(filecriteria.StatusMode==2)
            {
                data = data.Where(x => x.Shared).ToList();
            }
            if (filecriteria.FileMode == 2)
            {
                data = new List<FileUpload>();
            }
            return data;
        }
        public async Task<int> FormatAsync(long user)
        {
            return await _fileUploadRepository.FormatAsync(user);
        }
        public bool Setpassword(string Id, string Newpassword)
        {
            PasswordHash hash = new PasswordHash();
            if (!string.IsNullOrEmpty(Newpassword))
            {
                Newpassword = hash.CreateEncrypted(Id, Newpassword);
            }
            return _fileUploadRepository.Setpassword(Id, Newpassword);
        }
        public bool SetnamespaceAndShared(string reference,string namespaces,bool shared)
        {
            var item = _fileUploadRepository.GetFileByRef(reference);
            item.Uploadname = namespaces;
            item.Shared = shared;
            if(!string.IsNullOrEmpty(item.pass))
            {
                return _fileUploadRepository.VerifyUpdate(item,item.pass);
            }
            return _fileUploadRepository.Update(item);
        }
        public FileUpload GetByToken(string Token)
        {
            PasswordHash hash = new PasswordHash();
            Token = hash.DecodeFrom64(Token);
            return _fileUploadRepository.GetByToken(Token);
        }
       public decimal GetTotalUsedSpace()
        {
            return _fileUploadRepository.GetTotalUsedSpace();
        }

    }
}
