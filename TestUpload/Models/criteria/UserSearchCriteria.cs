using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload.Models.criteria
{
    public class UserSearchCriteria
    {
        public dt Brithday { get; set; }
        public dt Registerddate { get; set; }
        public int male { get; set; }
        public int Spaces { get; set; }
        public int Verify { get; set; }
        public int Suspension { get; set; }
    }
    public class dt
    {
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
    }
}
