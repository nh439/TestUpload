using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.View
{
    public class FileTotal
    {
        public string Id { get; set; }
         public string Filename{get; set;}
         public string FileNamespace{get; set;}
         public long UserId{get; set;}
         public decimal Filesize{get; set;}
         public string FileType{get; set;}
         public DateTime AddDate{get; set;}
         public bool HasPassword{get; set;}
         public bool Shared{get; set;}
        public string UploadId { get; set; }
    }
}
