using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Repository.SQL;
using TestUpload.Models.Entity;
using TestUpload.Securities;
using System.Data;
using System.Globalization;
using TestUpload.Models.View;
using TestUpload.Models.criteria;

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
        Task<List<User>> GetVerifiedAccountsAsync();
        Task<List<User>> GetSuspendAccountsAsync();
        Task<List<User>> GetUnSuspendAccountsAsync();
        int SetVerifyByadmin(long userId);
        User GetById(long id);
        Task<List<UserView>> GetViewModelAsync();
        Task<List<UserView>> getViewWithAdvancedSearch(UserSearchCriteria criteria);
    }
    public class userService : IuserService
    {
        private readonly UserRepository _userRepository;
        private readonly LoginRepository _loginRepository;
        private readonly ChangepassRepository _changepasswordRepository;
        public userService(UserRepository userRepository, LoginRepository loginRepository, ChangepassRepository changepasswordRepository)
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
            var Encypthash = passwordHash.Create(key, hash);
            item.Login.Password = Encypthash;
            return _userRepository.Create(item);
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
            return user;
        }
        public async Task<List<User>> GetallUsersAsync()
        {
            var users = await _userRepository.GetallAsync();
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Login = new Login();
                users[i].Login = _loginRepository.GetDataByUserId(users[i].Id);
            }

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
            foreach (var i in res)
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
            if (CurrentUser.Verify)
            {
                return 2;
            }
            user.VerifyBy = "Admin";
            user.VerifyDate = DateTime.Now;
            _userRepository.Update(user);
            CurrentUser.Verify = true;
            CurrentUser.Token = string.Empty;
            return _loginRepository.Update(CurrentUser) ? 1 : 0;
        }
        public async Task<List<User>> GetVerifiedAccountsAsync()
        {
            var data = await _userRepository.GetVerifiedAccountsAsync();
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Login = new Login();
                data[i].Login = _loginRepository.GetDataByUserId(data[i].Id);
            }
            return data;
        }
        public async Task<List<User>> GetSuspendAccountsAsync()
        {
            var users = await _userRepository.GetSuspendAccountsAsync();
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Login = new Login();
                users[i].Login = _loginRepository.GetDataByUserId(users[i].Id);
            }

            return users;
        }
        public async Task<List<User>> GetUnSuspendAccountsAsync()
        {
            var users = await _userRepository.GetUnSuspendAccountsAsync();
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Login = new Login();
                users[i].Login = _loginRepository.GetDataByUserId(users[i].Id);
            }

            return users;
        }
        public User GetById(long id)
        {
            var data = _userRepository.GetById(id);
            data.Login = _loginRepository.GetDataByUserId(id);
            return data;
        }
        public async Task<List<UserView>> GetViewModelAsync()
        {
            return await _userRepository.GetViewModelAsync();
        }
        public async Task<List<UserView>> getViewWithAdvancedSearch(UserSearchCriteria criteria)
        {
            var data = await _userRepository.GetViewModelAsync();
            if(criteria.Brithday.Startdate.HasValue)
            {
                data = data.Where(x => x.BrithDay.Date >= criteria.Brithday.Startdate.Value.Date).ToList();
            }
            if (criteria.Brithday.Enddate.HasValue)
            {
                data = data.Where(x => x.BrithDay.Date < criteria.Brithday.Enddate.Value.AddDays(1).Date).ToList();
            }
            if (criteria.Registerddate.Startdate.HasValue)
            {
                data = data.Where(x => x.Registerd.Date >= criteria.Registerddate.Startdate.Value.Date).ToList();
            }
            if (criteria.Registerddate.Enddate.HasValue)
            {
                data = data.Where(x => x.Registerd.Date < criteria.Registerddate.Enddate.Value.AddDays(1).Date).ToList();
            }
            if(criteria.Spaces >0)
            {
                decimal gb = 1024 * 1024 * 1024;
                switch (criteria.Spaces)
                {   
                    case 1:
                        data = data.Where(x => x.Used < 1048576).ToList();
                        break;
                    case 2:
                        data = data.Where(x => x.Used >= 1048576 && x.Used < gb).ToList();
                        break;
                    case 3:
                        decimal max = 10737418240;
                        data = data.Where(x => x.Used >= gb && x.Used < max).ToList();
                        break;
                    case 4:                        
                        decimal min = 10737418240;
                        decimal mx = 25*gb;
                        data = data.Where(x => x.Used >= min && x.Used < mx).ToList();
                        break;
                    case 5:
                        gb = 1024 * 1024 * 1024;
                        mx = 25 * gb;
                        data = data.Where(x => x.Used >= mx).ToList();
                        break;
                }
            }
            if(criteria.male > 0)
            {
                switch (criteria.male)
                {
                    case 1:
                        data = data.Where(x => x.Male).ToList();
                        break;
                    case 2:
                        data = data.Where(x => !x.Male).ToList();
                        break;
                }
            }
            if(criteria.Verify >0)
            {
                switch (criteria.Verify)
                {
                    case 1:
                        data = data.Where(x => x.Verify).ToList();
                        break;
                    case 2:
                        data = data.Where(x => !x.Verify).ToList();
                        break;
                }
            }
            if (criteria.Suspension > 0)
            {
                switch (criteria.Suspension)
                {
                    case 1:
                        data = data.Where(x => x.Suspend).ToList();
                        break;
                    case 2:
                        data = data.Where(x => !x.Suspend).ToList();
                        break;
                }
            }
            return data;
        }



    }
}
