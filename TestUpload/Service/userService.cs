using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Repository.SQL;
using TestUpload.Models.Entity;
using TestUpload.Securities;
using System.Data;
using System.Globalization;

namespace TestUpload.Service
{
    public interface IuserService
    {
         bool Register(User item);
         User GetWithoutPassword(long id);
         User GetByUsername(string Username);
         Task<List<User>> GetallUsersAsync();
         Task<DataTable> GetusernameEmailList();
         Task<List<User>> GetUnverifyAsync();
         int SetVerifyByadmin(long userId);
    }
    public class userService :IuserService
    {
        private readonly UserRepository _userRepository;
        private readonly LoginRepository _loginRepository;
        private readonly ChangepassRepository _changepasswordRepository;
        public userService(UserRepository userRepository,LoginRepository loginRepository,ChangepassRepository changepasswordRepository)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _changepasswordRepository = changepasswordRepository;

        }

        public bool Register(User item)
        {
            var hash = item.Login.Password;
            var key = item.Login.Username;
            PasswordHash passwordHash = new PasswordHash();
            var Encypthash = passwordHash.Create(key,hash);
            item.Login.Password = Encypthash;
            return  _userRepository.Create(item);
        }
        public User GetWithoutPassword(long id)
        {
            var user = _userRepository.GetById(id);
   //         user.Login.Password = "PASSWORD";
            return user;
        }
        public User GetByUsername(string Username)
        {
            var user = _userRepository.GetByUsername(Username);
            user.Login.Password = "PASSWORD";
            return user;
        }
       public async Task<List<User>> GetallUsersAsync()
        {
            var users = await  _userRepository.GetallAsync();
            
            return users;
        }
        public async Task<DataTable> GetusernameEmailList()
        {
            return await _userRepository.GetUsernameandEmailList();
        }
        public async Task<List<User>> GetUnverifyAsync()
        {
            var res = await _userRepository.GetUnverifyAccountsAsync();
            int h = 0;
            foreach(var i in res)
            {
                var j = _userRepository.GetById(i.Id);
                res[h].Login = new Login();
                h++;
            }
            return res;
        }
        public int SetVerifyByadmin(long userId)
        {
            User user = _userRepository.GetById(userId);
            Login CurrentUser = _loginRepository.GetDataByUserId(userId);
            if(CurrentUser.Verify)
            {
                return 2;
            }
            user.VerifyBy = "Admin";
            user.VerifyDate =DateTime.Now;
            _userRepository.Update(user);                    
            CurrentUser.Verify = true;
            CurrentUser.Token = string.Empty;
            return _loginRepository.Update(CurrentUser) ? 1:0;            
        }



    }
}
