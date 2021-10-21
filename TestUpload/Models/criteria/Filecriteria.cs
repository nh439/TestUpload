using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.criteria
{
    public class Filecriteria
    {
        public string FileExtension { get; set; }
        public DateTime? AddDateStarts { get; set; }
        public DateTime? AddDateEnd { get; set; }
        public string FileNamespace { get; set; }
        public bool HasPassword { get; set; }
        public int FileMode { get; set; }
        public int StatusMode { get; set; }
        public int? UserId { get; set; }

    }
}
