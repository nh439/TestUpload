using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.Entity
{
    public class History
    {
        [Key]
        public long Id { get; set; }
        public string Filename { get; set; }
        public string FileMode { get; set; }
        public long UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
