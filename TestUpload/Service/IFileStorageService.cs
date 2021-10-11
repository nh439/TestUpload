using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.criteria;
using TestUpload.Models.Entity;
using TestUpload.Models.View;
using TestUpload.Repository.SQL;
using TestUpload.Securities;

namespace TestUpload.Service
{
    public interface IFileStorageService
    {
        Task<int> CreateAsync(List<FileStorage> files);
        Task<List<FilestorageView>> GetAllFileStoragesAsync();
        Task<List<FilestorageView>> GetFilesByUserAsync(long user);
        Task<List<FilestorageView>> GetFilescriteria(long user, Filecriteria filecriteria);
        FileStorage Download(string reference);
        FileStorage VerifyDownload(string reference, string password);
        int Delete(List<string> references);
        bool DeleteOne(string reference);
        bool VerifyRemove(string reference, string password);
        Task<FilestorageView> GetViewById(string FileId);
        Task<int> FormatAsync(long user);
        bool Setpassword(string Id, string Newpassword);
        bool SetnamespaceAndShared(string reference, string namespaces, bool shared);
    }
    public class FileStorageService:IFileStorageService
    {
        private readonly FileStorageRepository _FileStorageRepository;
        public FileStorageService(FileStorageRepository FileStorageRepository)
        {
            _FileStorageRepository = FileStorageRepository;
        }
        public async Task<int> CreateAsync(List<FileStorage> files)
        {
            return await _FileStorageRepository.Create(files);
        }
        public async Task<List<FilestorageView>> GetAllFileStoragesAsync()
        {
            return await _FileStorageRepository.Getall();
        }
        public async Task<List<FilestorageView>> GetFilesByUserAsync(long user)
        {
            return await _FileStorageRepository.GetByUser(user);
        }
        public FileStorage Download(string reference)
        {
            return _FileStorageRepository.GetFile(reference);
        }
        public FileStorage VerifyDownload(string reference, string password)
        {
            return _FileStorageRepository.GetFile(reference, password);
        }
        public int Delete(List<string> references)
        {
            return _FileStorageRepository.Remove(references);
        }
        public bool VerifyRemove(string reference, string password)
        {
            return _FileStorageRepository.VerifyRemoved(reference, password);
        }
        public async Task<FilestorageView> GetViewById(string FileId)
        {
            return await _FileStorageRepository.GetById(FileId);
        }
        public bool DeleteOne(string reference)
        {
            return _FileStorageRepository.RemoveOne(reference);
            
        }

        public async Task<List<FilestorageView>> GetFilescriteria(long user, Filecriteria filecriteria)
        {
            var data = await _FileStorageRepository.GetByUser(user);
            if(!string.IsNullOrEmpty(filecriteria.FileExtension))
            {
                data = data.Where(x => x.FileExtension == filecriteria.FileExtension).ToList();
            }
            if(!string.IsNullOrEmpty(filecriteria.FileNamespace))
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
                data = data.Where(x => x.HasPassword).ToList();
            }
            if(filecriteria.AddDateStarts.HasValue)
            {
                data = data.Where(x => x.AddDate > filecriteria.AddDateStarts.Value.Date).ToList();
            }
            if (filecriteria.AddDateEnd.HasValue)
            {
                data = data.Where(x => x.AddDate <= filecriteria.AddDateEnd.Value.Date.AddDays(1)).ToList();
            }
            if (filecriteria.StatusMode == 1)
            {
                data = data.Where(x => !x.Shared).ToList();
            }
            if (filecriteria.StatusMode == 2)
            {
                data = data.Where(x => x.Shared).ToList();
            }
            if (filecriteria.FileMode==1)
            {
                data = new List<FilestorageView>();
            }
            return data;
        }
        public async Task<int> FormatAsync(long user)
        {
            return await _FileStorageRepository.FormatAsync(user);
        }
        public bool Setpassword(string Id, string Newpassword)
        {
            PasswordHash hash = new PasswordHash();
            if (!string.IsNullOrEmpty(Newpassword))
            {
                Newpassword = hash.CreateEncrypted(Id, Newpassword);
            }
            return _FileStorageRepository.Setpassword(Id, Newpassword);
        }
        public bool SetnamespaceAndShared(string reference, string namespaces, bool shared)
        {
            var item = _FileStorageRepository.GetFile(reference);
            item.Uploadname = namespaces;
            item.Shared = shared;
            if (!string.IsNullOrEmpty(item.pass))
            {
                return _FileStorageRepository.VerifyUpdate(item, item.pass);
            }
            return _FileStorageRepository.Update(item);
        }

    }

}
