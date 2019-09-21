using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class SAILING_DATA
    {
        public long ID { get; set; }
        
        public string CALLSIGN { get; set; }

        public DateTime TIME_STAMP { get; set; }

        public double LAT { get; set; }

        public double LON { get; set; }

        public double SPEED_VG { get; set; }


        public double SPEED_LW { get; set; }


        public double REL_WIND_DIR { get; set; }

        public double REL_WIND_SPEED { get; set; }

        public double COURSE_OVER_GROUND { get; set; }

        public double SHIP_HEADING { get; set; }

        public double WATER_DEPTH { get; set; }

        public double RUDDER_ANGLE { get; set; }

        public double DRAFT_FORE { get; set; }

        public double DRAFT_AFT { get; set; }

        public double ABS_WIND_DIR { get; set; }

        public double ABS_WIND_SPEED { get; set; }

        public double BHP_BY_FOC { get; set; }

        public double SLIP { get; set; }

        public double SHAFT_REV { get; set; }

        public double SHAFT_POWER { get; set; }

        public double SHAFT_TORQUE { get; set; }

        public double ME_FOC_HOUR { get; set; }

        public bool IN_PORT { get; set; }

        public bool IN_HARBOUR { get; set; }

        public bool AT_SEA { get; set; }

        public double TOTAL_WAVE_HEIGHT { get; set; }

        public double TOTAL_WAVE_DIRECTION { get; set; }

        public double TOTAL_WAVE_PERIOD { get; set; }

        public double WIND_WAVE_HEIGHT { get; set; }

        public double WIND_WAVE_DIRECTION { get; set; }

        public double WIND_WAVE_PERIOD { get; set; }

        public double SWELL_WAVE_HEIGHT { get; set; }

        public double SWELL_WAVE_DIRECTION { get; set; }

        public double SWELL_WAVE_PERIOD { get; set; }

        public double WIND_UV { get; set; }

        public double WIND_VV { get; set; }

        public double WIND_SPEED { get; set; }

        public double WIND_DIRECTION { get; set; }

        public double AMBIENT_TEMP { get; set; }

        public double AMBIENT_DENSITY { get; set; }

        public double PRESSURE_SURFACE { get; set; }

        public double PRESSURE_MSL { get; set; }

        public double SEA_SURFACE_SALINITY { get; set; }

        public double SEA_SURFACE_TEMP { get; set; }

        public double CURRENT_UV { get; set; }

        public double CURRENT_VV { get; set; }

        public double CURRENT_SPEED { get; set; }

        public double CURRENT_DIRECTION { get; set; }
        public double BN { get; set; }
        public bool IS_GOOD_DATA { get; set; }
        public bool IS_SHAFTMETER_GOOD_DATA { get; set; }        
        public bool VALID_CHAUVENT { get; set; }        
        public bool VALID_VALIDATION { get; set; }        
        public bool VALID_REFCONDITION { get; set; }

    }
}
