using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.criteria
{
    public class SessionCriteria
    {
        public dt LoggedIn { get; set; }
        public dt LoggedOut { get; set; }
        public long UserId { get; set; }
        public int LogoutState { get; set; }
    }
}
