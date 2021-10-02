using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;

namespace TestUpload.Repository.SQL
{
     
    public class ChangepassRepository
    {
        private readonly DataContext _context;
        public ChangepassRepository(DataContext context)
        {
            _context = context;
        }
        public bool Create(Changepassword item)
        {
             _context.changepassword.Add(item);
            var i = _context.SaveChanges();
            return i > 0 ? true : false;

        }
        public List<Changepassword> Getall()
        {
            return _context.changepassword.OrderByDescending(x=>x.ChangeDate).ToList();
        }
    }
}
