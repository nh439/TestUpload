using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.View
{
    public class UserView
    {
      
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }      
        public string Email { get; set; }
        public DateTime Registerd { get; set; } 
        public bool Admin { get; set; } = false;
        public string Username { get; set; }
        public bool Male { get; set; }
        public DateTime BrithDay { get; set; }
        public DateTime VerifyDate { get; set; }
        public string VerifyBy { get; set; }
        public bool Verify { get; set; }
        public bool Suspend { get; set; }
        public decimal? Used { get; set; }
    }
}
