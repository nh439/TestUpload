using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.View;
using TestUpload.Repository;

namespace TestUpload.Service
{
    public interface IFileTotalServices
    {
        Task<List<FileTotal>> GetAsync();
    }
    public class FileTotalServices:IFileTotalServices
    {
        private readonly FileTotalRepository _fileTotalRepository;
        public FileTotalServices(FileTotalRepository fileTotalRepository)
        {
            _fileTotalRepository = fileTotalRepository;
        }
        public async Task<List<FileTotal>> GetAsync()
        {
            return await _fileTotalRepository.GetFileTotalsAsync();
        }
    }
}
