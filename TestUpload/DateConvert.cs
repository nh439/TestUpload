using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUpload
{
    public static class DateConvert
    {

        public static DateTime GetBuddhist_era(DateTime Christ_era)
        {
            return Christ_era.AddYears(543);

        }
        public static DateTime GetChirst_era(DateTime Buddhist_era)
        {
            return Buddhist_era.AddYears(-543);
        }
    }
}
