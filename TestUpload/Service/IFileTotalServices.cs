using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.criteria;
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
        public async Task<List<FileTotal>> GrtByAdvancedSearch(Filecriteria filecriteria)
        {
            var data = await _fileTotalRepository.GetFileTotalsAsync();
            if (!string.IsNullOrEmpty(filecriteria.FileExtension))
            {
                data = data.Where(x => x.FileExtension == filecriteria.FileExtension).ToList();
            }
            if (!string.IsNullOrEmpty(filecriteria.FileNamespace))
            {
                if (filecriteria.FileNamespace == "[No Namespaces]")
                {
                    data = data.Where(x => x.FileNamespace == string.Empty).ToList();
                }
                else
                {
                    data = data.Where(x => x.FileNamespace == filecriteria.FileNamespace).ToList();
                }
            }

            if (filecriteria.HasPassword)
            {
                data = data.Where(x => x.HasPassword).ToList();
            }
            if (filecriteria.AddDateStarts.HasValue)
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
            if (filecriteria.FileMode == 1)
            {
                data = data.Where(x => x.FileType == "File").ToList();
            }
            if (filecriteria.FileMode == 2)
            {
                data = data.Where(x => x.FileType == "Blob").ToList();
            }
            return data;
        }
    }
}
