using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.View
{
    public class FilestorageView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long UserId { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public decimal FileSize { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool HasPassword { get; set; }
        public string Comment { get; set; }
    }
}
