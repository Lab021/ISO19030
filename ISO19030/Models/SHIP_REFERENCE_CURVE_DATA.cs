using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class SHIP_REFERENCE_CURVE_DATA
    {
        public long ID { get; set; }
        public string SHIP_KEY { get; set; }

        public string USER_ID_RESISTED { get; set; }

        public string NAME { get; set; }

        public double B_SPEED_TO_POWER { get; set; }

        public double A_SPEED_TO_POWER { get; set; }

        public double B_POWER_TO_SPEED { get; set; }

        public double A_POWER_TO_SPEED { get; set; }

        public double DRAFT_FORE { get; set; }

        public double DRAFT_AFT { get; set; }
    }    
}
