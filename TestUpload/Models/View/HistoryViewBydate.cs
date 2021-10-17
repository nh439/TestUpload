using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.View
{
    public class HistoryViewBydate
    {
        public DateTime Date { get; set; }
        public int Histories { get; set; }
        public int Success { get; set; }
    }
}
