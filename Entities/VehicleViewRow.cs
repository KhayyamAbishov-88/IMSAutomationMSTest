using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSAutomation.Entities
{
    internal class VehicleViewRow
    {
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string SubModel { get; set; }
        public int YearOld { get; set; } // year_old is the vehicle's age
    }
}
