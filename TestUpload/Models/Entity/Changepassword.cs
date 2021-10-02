using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.Entity
{
    public class Changepassword
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;
        public bool BySystem { get; set; } = false;
        public bool ResetPasswordRequire { get; set; } = false;


    }
}
