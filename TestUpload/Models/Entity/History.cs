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
        public string Detail { get; set; }
        public string HistoryMode { get; set; }
        public string RelatedFile { get; set; }
        public long UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Issuccess { get; set; }
        public string ErrorLog { get; set; }
        
    }

}
