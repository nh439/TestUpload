using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TestUpload.Models.Entity;

namespace TestUpload.Repository.SQL
{
    public class HistoryRepository
    {
        private readonly DataContext _context;
        public HistoryRepository(DataContext context)
        {
            _context = context;
        }
        public bool Create(History item)
        {
            _context.Add(item);
            var res = _context.SaveChanges();
            return res != 0 ? true : false;
        }
        public async Task<Dictionary<string,int>> Clear()
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            await _context.Database.BeginTransactionAsync();
            var errorlog =   await _context.Database.ExecuteSqlRawAsync("delete from errorlog");
            var history =  await  _context.Database.ExecuteSqlRawAsync("delete from history");
            await _context.Database.CommitTransactionAsync();
            res.Add("errors", errorlog);
            res.Add("history", history);
            return res;
          


        }
        public async Task<List<History> > Getall()
        {
            return await _context.History.OrderByDescending(x => x.Date).ToListAsync();
        }
        public async Task<List<History>> GetErrorHistory()
        {
            return await _context.History.Where(x => !x.Issuccess && x.ErrorLog != null).OrderByDescending(x => x.Date).ToListAsync();
        }
        public async Task<List<History>> GetSuccessHistory()
        {
            return await _context.History.Where(x => x.Issuccess && x.ErrorLog == null).OrderByDescending(x => x.Date).ToListAsync();
        }
        public async Task<List<History>> GetByUsername(long Id)
        {
            return await _context.History.Where(x => x.UserId==Id).OrderByDescending(x => x.Date).ToListAsync();
        }

    }
    public class ErrorLogRepository
    {
        private readonly DataContext _context;
        public ErrorLogRepository(DataContext context)
        {
            _context = context;
        }
        public bool create(ErrorLog item)
        {
            _context.Add(item);
            return _context.SaveChanges() > 0 ? true : false;
        }

    }
}
