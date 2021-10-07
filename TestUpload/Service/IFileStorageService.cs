using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    }

}
