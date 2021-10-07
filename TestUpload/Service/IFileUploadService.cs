using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Repository.SQL;

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


    }
}
