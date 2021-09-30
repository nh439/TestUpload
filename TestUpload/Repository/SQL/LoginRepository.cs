using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;

namespace TestUpload.Repository.SQL
{
    public class LoginRepository
    {
        private readonly DataContext _context;
        public LoginRepository(DataContext context)
        {
            _context = context;
        }
        public bool Create(Login item)
        {
             _context.login.Add(item);
            var res = _context.SaveChanges();
            if(res > 0)
            {
                return true;
            }
            return false;
        }
        public bool GetLogin(string Username,String Password)
        {
            var res = _context.login.Where(x => x.Username == Username && x.Password == Password).ToList();
            if(res.Count==1)
            {
                return true;
            }
            return false;
        }

    }
}
