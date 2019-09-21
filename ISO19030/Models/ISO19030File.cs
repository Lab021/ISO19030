using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class ISO19030File
    {
        public int ID { get; set; }
        public DateTime TIME_STAMP { get; set; }
        public double SPEED_VG { get; set; }
        public double SPEED_LW { get; set; }
        public double REL_WIND_DIR { get; set; }
        public double REL_WIND_SPEED { get; set; }
        public double SHIP_HEADING { get; set; }
        public double WATER_DEPTH { get; set; }
        public double RUDDER_ANGLE { get; set; }
        public double DRAFT_FORE { get; set; }
        public double DRAFT_AFT { get; set; }
        public double ME1_RPM_SHAFT { get; set; }
        public double ME1_SHAFT_POWER { get; set; }
        public bool VALID_CHAUVENT { get; set; }
        public bool VALID_REFCONDITION { get; set; }
        public bool VALID_VALIDATION { get; set; }
        public double SW_TEMP { get; set; }
        public double AMBIENT_DENSITY { get; set; }
        public double AMBIENT_TEMP { get; set; }
    }
}
