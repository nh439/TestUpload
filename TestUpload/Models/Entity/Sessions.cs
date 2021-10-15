using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.Entity
{
    public class Sessions
    {
        [Key]
        public string Id { get; set; } = StrRandom.RandomString(100);
        public long UserId { get; set; }
        public DateTime LoggedIn { get; set; }
        public DateTime? Loggedout { get; set; }
        public string IpAddress { get; set; }
        public bool IsLogout { get; set; } 
    }
}
