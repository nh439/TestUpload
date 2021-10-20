using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TestUpload.Models.Entity;
using TestUpload.Models.View;

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
        public async Task<int> Clear()
        {
            await _context.Database.BeginTransactionAsync();
            var history =  await  _context.Database.ExecuteSqlRawAsync("delete from history");
            await _context.Database.CommitTransactionAsync();
            return history;       
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
        public async Task<List<HistoryViewBydate>> GetViewBydate()
        {
            var data = (from histories in _context.History
                        group histories by histories.Date.Date
                       into datefilter
                        select new HistoryViewBydate
                        {
                            Date = datefilter.Key,
                            Histories = datefilter.Count(),
                            Success = datefilter.Count(x => x.Issuccess)

                        }
                       );
            return await data.OrderByDescending(x => x.Date).ToListAsync();
        }
        public async Task<int> Clear(int Month)
        { 
           return await _context.Database.ExecuteSqlRawAsync(string.Format( "delete from history where TIMESTAMPDIFF(month,date,current_timestamp())>= {0}",Month));
        }

    }
    

}
