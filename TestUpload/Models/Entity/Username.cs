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
        public string Rules { get; set; } = "User";
        public virtual Login Login { get; set; }

    }
    public class Login
    {
        [Key]
        public string Username { get; set; }
        [Required]
        [StringLength(514)]
        public string Password { get; set; }
    }
}
