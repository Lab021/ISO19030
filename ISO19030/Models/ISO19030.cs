using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class ISO19030RESULT
    {
        public DateTime START_TIME { get; set; }
        public DateTime END_TIME { get; set; }
        public double SPEED_VG { get; set; }
        public double SPEED_LW { get; set; }
        public double SHAFT_POWER { get; set; }
        public double SHAFT_REV { get; set; }

        public double DRAFT_FORE { get; set; }
        public double DRAFT_AFT { get; set; }
        public double PV { get; set; }
        public double PPV { get; set; }
        public double SLIP { get; set; }
        public double TOTAL_DATA_COUNT { get; set; }
        public double FILTER_DATA_COUNT { get; set; }

        public double CORRECT_POWER { get; set; }
        public double VE { get; set; }
    }
}
