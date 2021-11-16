using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Service
{
   public interface ISessionLogoutService
    {
        void AutoLogout();
    }
    public class SessionLogoutService:ISessionLogoutService
    {
        private readonly DataContext _context;
        private readonly ILogger<SessionLogoutService> _logger;
        public SessionLogoutService(IConfiguration configuration, ILogger<SessionLogoutService> logger)
        {
            var constr = configuration.GetConnectionString("Default");
            var Optionbulider = new DbContextOptionsBuilder<DataContext>();
            Optionbulider.UseMySQL(constr);
            _context = new DataContext(Optionbulider.Options);
            _logger = logger;
        }
        public void AutoLogout()
        {
            try
            {
                string query = "update sessions set Loggedout=current_timestamp(),IsLogout=1  where timestampdiff(day,LoggedIn,current_timestamp()) >=1 and Loggedout is null;";
                var i= _context.Database.ExecuteSqlRaw(query);
                query = "delete from sessions where timestampdiff(month,loggedin,current_timestamp()) > 9";
                _context.Database.ExecuteSqlRaw(query);
            }
            catch(Exception x)
            {
                _logger.LogError(x.Message);
            }

        }
    }
}
