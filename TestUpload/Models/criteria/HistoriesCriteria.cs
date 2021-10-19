using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.criteria
{
    public class HistoriesCriteria
    {
        public dt Historiesdate { get; set; }
        public string Section { get; set; }
        public long Users { get; set; }
        public int State { get; set; }
    }
}
