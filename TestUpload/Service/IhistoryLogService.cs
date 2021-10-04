using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Repository.SQL;

namespace TestUpload.Service
{
    public interface IhistoryLogService
    {
        public bool CreateSuccessHistory(string Mode, string Details, string? RelatedFiles, long UserId);
        public string CreateErrorHistory(string Mode, string Details, string? RelatedFiles, long UserId, string Exception, string InnerException);
    }
    public class historyLogService :IhistoryLogService
    {
        private readonly HistoryRepository _historyRepository;
        private readonly ErrorLogRepository _errorLogRepository;
        public historyLogService(HistoryRepository historyRepository,ErrorLogRepository errorLogRepository)
        {
            _historyRepository = historyRepository;
            _errorLogRepository = errorLogRepository;
        }
        public bool CreateSuccessHistory(string Mode,string Details,string? RelatedFiles,long UserId)
        {
            History history = new History
            {
                Detail = Details,
                HistoryMode = Mode,
                Issuccess = true,
                RelatedFile = !string.IsNullOrEmpty(RelatedFiles) ? RelatedFiles : null,
                UserId = UserId
            };
            return _historyRepository.Create(history);

        }
        public string CreateErrorHistory(string Mode, string Details, string? RelatedFiles, long UserId,string Exception,string InnerException)
        {
            History history = new History
            {
                Detail = Details,
                HistoryMode = Mode,
                Issuccess = false,
                RelatedFile = !string.IsNullOrEmpty(RelatedFiles) ? RelatedFiles : null,
                UserId = UserId
                
            };
            ErrorLog log = new ErrorLog { ExceptionMessage = Exception, InnerException = InnerException };
            history.ErrorLog = log;
            _historyRepository.Create(history);
            return log.Reference;
        }
       


    }
}
