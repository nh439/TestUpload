using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;

namespace TestUpload.Profile
{
    public static class CurrentLogin
    {
        public static long Id { get; set; }
        public static string Firstname { get; set; }
        public static string Lastname { get; set; }
        public static string Email { get; set; }
        public static DateTime Registerd { get; set; } = DateTime.Now;
        public static string Rules { get; set; } = "User";
        public static bool Male { get; set; }

        public static DateTime BrithDay { get; set; }

        public static string SessionId { get; set; }
        public static string Username { get; set; }
    }
}
