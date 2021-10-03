using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Repository.SQL;
using TestUpload.Models.Entity;
using TestUpload.Securities;

namespace TestUpload.Service
{
    public interface IuserService
    {
         bool Register(User item);
         User GetWithoutPassword(long id);
         User GetByUsername(string Username);
         Task<List<User>> GetallUsersAsync();
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
            user.Login.Password = "PASSWORD";
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



    }
}
