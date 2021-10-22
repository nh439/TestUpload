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
    public interface ISessionServices
    {
        bool Logout(string SessionId);
        string Login(long UserId, string Ip);
        Task<List<Sessions>> GetallAsync();
        List<Sessions> GetByUser(long user);
        bool Sessioncheck(string SessionId);
        int ForcedCheckout(long user, string ExceptSessionId);

    }
    public class SessionServices :ISessionServices
    {
        private readonly SessionRepository _sessionRepository;
        public SessionServices(SessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }
        public string Login(long UserId,string Ip) 
        {
            Sessions sessions = new Sessions
            {
                IpAddress = Ip,
                UserId = UserId,
                LoggedIn = DateTime.Now,
                IsLogout = false
            };
            _sessionRepository.Create(sessions);
            return sessions.Id;
        }
        public bool Logout(string SessionId)
        {
            Sessions item = _sessionRepository.GetById(SessionId);
            item.IsLogout = true;
            item.Loggedout = DateTime.Now;
            return _sessionRepository.Update(item);
        }
        public async Task<List<Sessions>> GetallAsync()
        {
            return await _sessionRepository.Getall();
        }
        public List<Sessions> GetByUser(long user)
        {
            return _sessionRepository.GetByUser(user);
        }
        public int ForcedCheckout(long user, string ExceptSessionId)
        {
            return _sessionRepository.ForcedClear(user, ExceptSessionId);
        }
        public bool Sessioncheck(string SessionId)
        {
            return _sessionRepository.Sessioncheck(SessionId);
        }
        public async Task<List<Sessions>> AdvancedSearch(SessionCriteria criteria)
        {
            var data = await _sessionRepository.Getall();
            if(criteria.LoggedIn.Startdate.HasValue)
            {
                data = data.Where(x => x.LoggedIn >= criteria.LoggedIn.Startdate.Value.Date).ToList();
            }
            if(criteria.LoggedIn.Enddate.HasValue)
            {
                data = data.Where(x => x.LoggedIn < criteria.LoggedIn.Enddate.Value.Date).ToList();
            }
            if (criteria.LoggedOut.Startdate.HasValue)
            {
                data = data.Where(x => x.Loggedout >= criteria.LoggedOut.Startdate.Value.Date).ToList();
            }
            if (criteria.LoggedOut.Enddate.HasValue)
            {
                data = data.Where(x => x.Loggedout < criteria.LoggedOut.Enddate.Value.Date).ToList();
            }
            if(criteria.UserId >0)
            {
                data = data.Where(x => x.UserId == criteria.UserId).ToList();
            }
            if(criteria.LogoutState==1)
            {
                data = data.Where(x => x.IsLogout).ToList();
            }
            if(criteria.LogoutState==2)
            {
                data.Where(x => !x.IsLogout).ToList();
            }
            return data;

        }
    }
}
