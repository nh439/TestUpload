using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Repository.SQL;
using TestUpload.Securities;

namespace TestUpload.Service
{
    public interface ILoginService
    {
         User GetLogin(string username, string password);
        public Login GetDataByUserId(long Id);
        bool ChangeSuspendStatus(String username);
        int Changepassword(string username, string NewPassword, string OldPassword);
    }
    public class LoginService :ILoginService
    {
        private readonly LoginRepository _loginRepository;
        private readonly UserRepository _userRepository;
        private readonly ChangepassRepository _changepassRepository;
        public LoginService(LoginRepository loginRepository,UserRepository UserRepository,ChangepassRepository changepassRepository)
        {
            _loginRepository = loginRepository;
            _userRepository = UserRepository;
            _changepassRepository = changepassRepository;
        }
        public User GetLogin(string username,string password)
        {
            PasswordHash passwordHash = new PasswordHash();
            password = passwordHash.Create(username, password);
            bool t = _loginRepository.GetLogin(username, password);
            if(t)
            {
                var res = _userRepository.GetByUsername(username);
                res.Login.Password = "PASSWORD";
                return res;
            }
            return null;
        }
        public Login GetDataByUserId(long Id)
        {
            return _loginRepository.GetDataByUserId(Id);
        }
        public bool ChangeSuspendStatus(String username)
        {
            var item = _loginRepository.GetById(username);
            item.Suspend = !item.Suspend;
            return _loginRepository.Update(item);
        }
        public int Changepassword(string username,string NewPassword,string OldPassword)
        {
            var item = _loginRepository.GetById(username);
            PasswordHash hash = new PasswordHash();
            OldPassword = hash.Create(username, OldPassword);
            if(item != null)
            {
                if(OldPassword==item.Password)
                {
                    long UserId = _userRepository.GetByUsername(username).Id;
                    NewPassword = hash.Create(username, NewPassword);
                    item.Password = NewPassword;
                    var res = _loginRepository.Update(item);
                    _changepassRepository.Create(new Models.Entity.Changepassword
                    {
                        BySystem = false,
                        ResetPasswordRequire = false,
                        UserId=UserId
                    }) ;
                    return res ? 1 : 3;
                }
                return 2;
            }
            return 0;
        }
    }
}
