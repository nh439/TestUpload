using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.Entity
{
    public class FileUpload
    {
        [Key]      
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public long UserId { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public decimal FileSize { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime LastUpdate { get; set; }
        [StringLength(255)]
        public string pass { get; set; }
        public string Comment { get; set; }

    }
}
