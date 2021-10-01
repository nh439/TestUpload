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
        public bool Register(User item);
    }
    public class userService :IuserService
    {
        private readonly UserRepository _userRepository;
        private readonly LoginRepository _loginRepository;
        public userService(UserRepository userRepository,LoginRepository loginRepository)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
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



    }
}
