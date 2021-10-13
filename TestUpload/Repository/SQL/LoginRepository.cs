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
        public bool Update(Login item)
        {
            _context.login.Update(item);
            return _context.SaveChanges() > 0 ? true : false;
        }
        public Login GetById(string username)
        {
            return _context.login.Where(x => x.Username == username).FirstOrDefault();
        }
        public Login GetDataByUserId(long Id)
        {
            var data = _context.User.Join(
                _context.login,
                User => User.Login.Username,
                Login => Login.Username,
                (User, Login) => new
                {
                    UserId = User.Id,
                    Username = Login.Username,
                    Suspend = Login.Suspend,
                    Token = Login.Token,
                    Verify = Login.Verify,
                    Password = Login.Password
                }
                ).Where(x => x.UserId == Id).FirstOrDefault();
            Login res = new Login {
                Password = data.Password,
                Verify = data.Verify,
                Suspend = data.Suspend,
                Token = data.Token,
                Username = data.Username
            };
            return res;

        }
    }
}
