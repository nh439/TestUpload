using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.View;
using TestUpload.Repository.SQL;

namespace TestUpload.Repository
{
    public class FileTotalRepository
    {
        private readonly FileStorageRepository _fileStorageRepository;
        private readonly FileUploadRepository _fileUploadRepository;
        public FileTotalRepository(FileUploadRepository fileUploadRepository,FileStorageRepository fileStorageRepository)
        {
            _fileStorageRepository = fileStorageRepository;
            _fileUploadRepository = fileUploadRepository;
        }
        public async Task<List<FileTotal>> GetFileTotalsAsync()
        {
            List<FileTotal> fileTotals = new List<FileTotal>();
            var files = await _fileUploadRepository.Getall();
            var blob = await _fileStorageRepository.Getall();
            foreach(var i in files)
            {
                fileTotals.Add(new FileTotal
                {
                    AddDate = i.AddDate,
                    Filename = i.Filename + i.FileExtension,
                    FileNamespace = i.Uploadname,
                    Filesize = i.FileSize,
                    FileType = "File",
                    HasPassword = !string.IsNullOrEmpty(i.pass) ? true : false,
                    Shared = i.Shared,
                    UploadId = i.UploadId,
                    UserId = i.UserId
                });
            }
            foreach(var i in blob)
            {
                fileTotals.Add(new FileTotal
                {
                    AddDate = i.AddDate,
                    Filename = i.Filename + i.FileExtension,
                    FileNamespace = i.Uploadname,
                    Filesize = i.FileSize,
                    FileType = "Blob",
                    HasPassword = i.HasPassword,
                    Shared = i.Shared,
                    UploadId = i.UploadId,
                    UserId = i.UserId
                });
            }
            return fileTotals.OrderBy(x => x.FileNamespace).OrderBy(x => x.UserId).ToList();
        }
    }
}
