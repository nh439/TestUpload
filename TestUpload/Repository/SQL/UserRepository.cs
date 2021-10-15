using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using Microsoft.EntityFrameworkCore.Query;
using TestUpload.Models.View;

namespace TestUpload.Repository.SQL
{
    public class UserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public bool Create(User item)
        {
            _context.User.Add(item);
            var res = _context.SaveChanges();
            if(res>0)
            {
                return true;
            }
            return false;
        }
        public User GetById(long id)
        {
            var data = _context.User.Where(x => x.Id == id).FirstOrDefault();
            return data;
        }
        public User GetByUsername(String Username)
        {
            return _context.User.Where(x => x.Login.Username == Username).FirstOrDefault();
        }
        public async Task<List<User>> GetallAsync()
        {
            var data = await _context.User.OrderBy(x => x.Registerd).ToListAsync(); ;
            return data;
        }
        public async  Task<DataTable> GetUsernameandEmailList()
        {
            var user = await (from u in _context.User
                        select new
                        {
                            u.Login.Username,
                            u.Email
                        }
                        ).ToListAsync();
            DataTable table = new DataTable();
            table.Columns.Add("Username");
            table.Columns.Add("Email");
            foreach(var i in user)
            {
                table.Rows.Add(i.Username, i.Email);
            }
            return table;
        }
        public async Task<List<User>> GetUnverifyAccountsAsync()
        {
            return await _context.User.Where(x => !x.Login.Verify && !x.Admin).ToListAsync();
        }
        public bool Update(User item)
        {
            _context.User.Update(item);
            return _context.SaveChanges() > 0 ? true : false;
        }
        public async Task<List<User>> GetVerifiedAccountsAsync()
        {
            return await _context.User.Where(x => x.Login.Verify && !x.Admin).ToListAsync();
        }
        public async Task<List<User>> GetSuspendAccountsAsync()
        {
            return await _context.User.Where(x => x.Login.Suspend && !x.Admin).ToListAsync();
        }
        public async Task<List<User>> GetUnSuspendAccountsAsync()
        {
            return await _context.User.Where(x => !x.Login.Suspend && !x.Admin).ToListAsync();
        }
        public async Task<List<UserView>> GetViewModelAsync()
        {
            var raw = await (from UserProfile in _context.User
                       join
UserLogin in _context.login
on UserProfile.Login.Username equals UserLogin.Username
                       select new UserView
                       {
                           Admin=UserProfile.Admin,
                           BrithDay=UserProfile.BrithDay,
                           Email=UserProfile.Email,
                           Firstname=UserProfile.Firstname,
                           Id=UserProfile.Id,
                           Lastname=UserProfile.Lastname,
                           Male=UserProfile.Male,
                           Registerd=UserProfile.Registerd,
                           Suspend=UserLogin.Suspend,
                          // UsedFile= _context.fileUploads.Where(x=>x.UserId == UserProfile.Id) != null? _context.fileUploads.Where(x => x.UserId == UserProfile.Id).Select(x => x.FileSize).ToList().Sum():0,
                          // UsedStorage = _context.fileStorage.Where(x => x.UserId == UserProfile.Id) != null? _context.fileStorage.Where(x=>x.UserId==UserProfile.Id).Select(x=>x.FileSize).ToList().Sum() :0,
                           Username=UserLogin.Username,
                           Verify=UserLogin.Verify,
                           VerifyBy=UserProfile.VerifyBy,
                           VerifyDate=UserProfile.VerifyDate
                       }
                      ).ToListAsync();     
            foreach(var i in raw)
            {
                int idx = raw.IndexOf(i);
                var file = _context.fileUploads.Where(x => x.UserId == i.Id) != null ? _context.fileUploads.Where(x => x.UserId == i.Id).Select(x => x.FileSize).ToList().Sum() : 0;
                var blob = _context.fileStorage.Where(x => x.UserId == i.Id) != null ? _context.fileStorage.Where(x => x.UserId == i.Id).Select(x => x.FileSize).ToList().Sum() : 0;
                var used = file + blob;
                raw[idx].Used = used;
            }
            return raw;
        }


    }
}
