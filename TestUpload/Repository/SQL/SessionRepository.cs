using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace TestUpload.Repository.SQL
{
    public class SessionRepository
    {
        private readonly DataContext _context;
        public SessionRepository(DataContext context)
        {
            _context = context;
        }
        public bool Create(Sessions item)
        {
            _context.sessions.Add(item);
            return _context.SaveChanges() > 0 ? true : false;
        }
        public async Task< List<Sessions>> Getall()
        {
            return await _context.sessions.OrderByDescending(x => x.LoggedIn).ToListAsync();
        }
        public List<Sessions> GetByUser(long user)
        {
            return _context.sessions.Where(x=>x.UserId==user).OrderByDescending(x => x.LoggedIn).ToList();
        }
        public bool Update(Sessions item)
        {
            _context.sessions.Update(item);
            return _context.SaveChanges() > 0 ? true : false;
        }
        public async Task<int> Removeall()
        {
            return await _context.Database.ExecuteSqlRawAsync("delete from sessions;");
        }
        public async Task<int> RemoveOverXMonth(int month)
        {
            return await _context.Database.ExecuteSqlRawAsync(string.Format("delete from sessions where timestampdiff(month,LoggedIn,current_timestamp) >= {0} and Loggedout is not null;", month));
        }
        public Sessions GetById(string Id)
        {
            return _context.sessions.Where(x => x.Id == Id).FirstOrDefault();
        }
        public int ForcedClear(long user, string ExceptSessionId)
        {
            var clear = _context.sessions.Where(x => !x.IsLogout && x.UserId == user && x.Id != ExceptSessionId).ToArray();
            for(int i =0;i<clear.Length;i++)
            {
                clear[i].IsLogout = true;
                clear[i].Loggedout = DateTime.Now;
                _context.sessions.Update(clear[i]);
            }
            return _context.SaveChanges();

        }
        public bool Sessioncheck(string SessionId)
        {
            var item = _context.sessions.Where(x => x.Id == SessionId && x.Loggedout == null).FirstOrDefault();
            if(item!=null)
            {
                return true;
            }
            return false;

        }
    }
}
 