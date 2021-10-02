using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Repository.SQL;

namespace TestUpload.Service
{
    public interface IChangepasswordService
    {
        bool Create(Changepassword item);
        List<Changepassword> Getall();
        List<Changepassword> GetByUser(long id);
        Changepassword GetLastestChanged(long Userid);
    }
    public class ChangepasswordService :IChangepasswordService
    {
        private readonly ChangepassRepository _changepassRepository;
        public ChangepasswordService(ChangepassRepository changepassRepository)
        {
            _changepassRepository = changepassRepository;
        }
        public bool Create(Changepassword item)
        {
            return _changepassRepository.Create(item);
        }
        public List<Changepassword> Getall()
        {
            return _changepassRepository.Getall();
        }
        public List<Changepassword> GetByUser(long id)
        {
            return _changepassRepository.Getall().Where(x => x.UserId == id).ToList();
        }
        public Changepassword GetLastestChanged(long Userid)
        {
            var data = _changepassRepository.Getall().Where(x => x.UserId == Userid);
            return data.OrderByDescending(x => x.ChangeDate).First();
        }
    }
}
