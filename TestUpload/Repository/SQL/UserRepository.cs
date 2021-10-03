using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;

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


    }
}
