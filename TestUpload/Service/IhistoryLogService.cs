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
    public interface IhistoryLogService
    {
        public bool CreateSuccessHistory(string Mode, string Details, string? RelatedFiles, long UserId);
        public string CreateErrorHistory(string Mode, string Details, string? RelatedFiles, long UserId, string Exception, string InnerException);
        Task<List<History>> Getall();
        Task<List<HistoryViewBydate>> GetViewBydate();
        Task<List<History>> AdvancedSearch(HistoriesCriteria criteria);
        Task<int> Clear(int Month);
        Task<int> Clear();
        Task<List<History>> GetbyUserAsync(long Id);
    }
    public class historyLogService :IhistoryLogService
    {
        private readonly HistoryRepository _historyRepository;
        public historyLogService(HistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
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
                UserId = UserId,
                ErrorLog=Exception
                
            };        

            _historyRepository.Create(history);
            return Exception;
        }
        public async Task<List<History>> Getall()
        {
            return await _historyRepository.Getall();
        }
        public async Task<List<HistoryViewBydate>> GetViewBydate()
        {
            return await _historyRepository.GetViewBydate();
        }
        public async Task<List<History>> AdvancedSearch(HistoriesCriteria criteria)
        {
            var data = await _historyRepository.Getall();
            if(criteria.Historiesdate.Enddate.HasValue)
            {
                data = data.Where(x => x.Date <= criteria.Historiesdate.Enddate.Value.Date.AddDays(1)).ToList();
            }
            if(criteria.Historiesdate.Startdate.HasValue)
            {
                data = data.Where(x => x.Date > criteria.Historiesdate.Startdate.Value.Date).ToList();
            }
            if(!string.IsNullOrEmpty(criteria.Section))
            {
                data = data.Where(x => x.HistoryMode == criteria.Section).ToList();
            }
            if(criteria.State!=0)
            {
                if(criteria.State ==1)
                {
                    data = data.Where(x => x.Issuccess).ToList();
                }
                if (criteria.State == 2)
                {
                    data = data.Where(x => !x.Issuccess).ToList();
                }
            }
            if(criteria.Users >0)
            {
                data = data.Where(x => x.UserId == criteria.Users).ToList();               
            }
            return data;
        }
        public async Task<int> Clear(int Month)
        {
            return await _historyRepository.Clear(Month);
        }
        public async Task<int> Clear()
        {
            return await _historyRepository.Clear();
        }
        public async Task<List<History>> GetbyUserAsync(long Id)
        {
            return await _historyRepository.GetByUsername(Id);
        }




    }
}
