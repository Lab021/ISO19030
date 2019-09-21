using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class checktemp
    {
        public DateTime TIMESTAMP { get; set; }

        public float DATA { get; set; }
        public bool SPEED_LW { get; set; }
        public bool SPEED_VG { get; set; }
        public bool BHP_BY_FOC { get; set; }
        public bool SHAFT_POWER { get; set; }
        public bool SHAFT_REV { get; set; }
        public bool REL_WIND_SPEED { get; set; }
        public bool REL_WIND_DIR { get; set; }
        public bool SHIP_HEADING { get; set; }
        public bool RUDDER_ANGLE { get; set; }
        public bool WATER_DEPTH { get; set; }
        public bool DRAFT_FORE { get; set; }
        public bool DRAFT_AFT { get; set; }
        public bool SW_TEMP { get; set; }
        public bool AIR_TEMP { get; set; }
        public bool SPEED_VG_VAL { get; set; }
        public bool SPEED_LW_VAL { get; set; }
        public bool SPEED_RPM_VAL { get; set; }
        public bool RUDDER_ANGLE_VAL { get; set; }
    }
}
