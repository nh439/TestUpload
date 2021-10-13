using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace TestUpload.Models.Entity
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Required]
        public string Email { get; set; }
        public DateTime Registerd { get; set; } = DateTime.Now;
        public bool Admin { get; set; } = false;
        public virtual Login Login { get; set; }

        [Required]
        public bool Male { get; set; }

        public DateTime BrithDay { get; set; }
        public DateTime VerifyDate { get; set; }
        public string VerifyBy { get; set; }

    }
    public class Login
    {
        [Key]
        public string Username { get; set; }
        [Required]
        [StringLength(514)]
        public string Password { get; set; }
        public bool Suspend { get; set; } = false;
        public bool Verify { get; set; } = false;
        public string Token { get; set; } = StrRandom.RandomString(52);
    }
}
