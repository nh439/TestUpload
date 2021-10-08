using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.criteria;
using TestUpload.Models.Entity;
using TestUpload.Models.View;
using TestUpload.Repository.SQL;

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
            if(!string.IsNullOrEmpty(filecriteria.Contentype))
            {
                data = data.Where(x => x.FileType == filecriteria.Contentype).ToList();
            }
            if(filecriteria.HasPassword)
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
            if(filecriteria.FileMode==1)
            {
                data = new List<FilestorageView>();
            }
            return data;
        }

    }

}
