using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        int ForcedClear(long user, string ExceptSessionId);

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
        public int ForcedClear(long user, string ExceptSessionId)
        {
            return _sessionRepository.ForcedClear(user, ExceptSessionId);
        }
        public bool Sessioncheck(string SessionId)
        {
            return _sessionRepository.Sessioncheck(SessionId);
        }
    }
}
